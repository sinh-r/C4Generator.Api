using C4Generator.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace C4Generator.Worker.Pipeline.Stages;

/// <summary>
/// Stage 4: Sends extracted code metadata to the Gemini API.
/// Expects a structured JSON response matching the C4Model schema.
/// Also extracts architectural insights from a second AI prompt.
/// </summary>
public sealed class AIInferenceStage : IArchitecturePipelineStage
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;
    private readonly ILogger<AIInferenceStage> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AIInferenceStage(HttpClient httpClient, IOptions<GeminiSettings> settings, ILogger<AIInferenceStage> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        if (context.ExtractedFiles.Count == 0)
        {
            _logger.LogWarning("No extracted files — skipping AI inference");
            context.C4ModelResult = BuildEmptyModel();
            context.Insights = [];
            return;
        }

        var codeSummary = BuildCodeSummary(context.ExtractedFiles);
        _logger.LogInformation("Calling Gemini for C4 model inference ({FileCount} files summarised)", context.ExtractedFiles.Count);

        context.C4ModelResult = await InferC4ModelAsync(codeSummary, cancellationToken);
        context.Insights = await InferInsightsAsync(codeSummary, cancellationToken);

        _logger.LogInformation("AI inference complete. Insights: {Count}", context.Insights.Count);
    }

    // ── C4 Model inference ────────────────────────────────────────────────────

    private async Task<C4Model> InferC4ModelAsync(string codeSummary, CancellationToken cancellationToken)
    {
        var prompt = $$"""
            You are a software architect. Analyse the following C# codebase summary and produce a C4 architecture model in valid JSON.

            CODEBASE SUMMARY:
            {{codeSummary}}

            OUTPUT FORMAT — respond ONLY with valid JSON matching this exact schema, no markdown, no explanation:
            {
              "Context": {
                "SystemName": "string",
                "SystemDescription": "string",
                "Actors": [{"Id":"string","Name":"string","Description":"string"}],
                "ExternalSystems": [{"Id":"string","Name":"string","Description":"string"}],
                "Relationships": [{"FromId":"string","ToId":"string","Label":"string","Technology":"string"}]
              },
              "Containers": {
                "Containers": [{"Id":"string","Name":"string","Technology":"string","Description":"string","Type":"Api|Database|Worker|UI|Queue|Other"}],
                "Relationships": [{"FromId":"string","ToId":"string","Label":"string","Technology":"string"}]
              },
              "Components": {
                "Components": [{"Id":"string","ContainerId":"string","Name":"string","Technology":"string","Description":"string","Layer":"Controller|Service|Repository|Handler|Other"}],
                "Relationships": [{"FromId":"string","ToId":"string","Label":"string","Technology":"string"}]
              },
              "Classes": {
                "Classes": [{"Id":"string","ComponentId":"string","Name":"string","Namespace":"string","Kind":"Class|Interface|Record|Enum","Methods":["string"]}],
                "Relationships": [{"FromId":"string","ToId":"string","Label":"string"}]
              }
            }
            """;

        var json = await CallGeminiAsync(prompt, cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<C4Model>(json, JsonOptions)
                ?? throw new InvalidOperationException("Gemini returned null for C4 model.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse C4 model JSON from Gemini: {Json}", json[..Math.Min(500, json.Length)]);
            return BuildEmptyModel();
        }
    }

    // ── Insight inference ─────────────────────────────────────────────────────

    private async Task<IReadOnlyList<ExtractedInsight>> InferInsightsAsync(string codeSummary, CancellationToken cancellationToken)
    {
        var prompt = $$"""
            You are a software architect. Analyse the following C# codebase summary and identify architectural insights such as high coupling, dependency cycles, layer violations, or areas for improvement.

            CODEBASE SUMMARY:
            {{codeSummary}}

            OUTPUT FORMAT — respond ONLY with a JSON array, no markdown, no explanation:
            [
              {"Category":"string","Title":"string","Description":"string","Severity":"Low|Medium|High|Critical"}
            ]
            """;

        var json = await CallGeminiAsync(prompt, cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<List<ExtractedInsight>>(json, JsonOptions)?.AsReadOnly()
                ?? (IReadOnlyList<ExtractedInsight>)[];
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse insights JSON from Gemini");
            return [];
        }
    }

    // ── Gemini HTTP call ──────────────────────────────────────────────────────

    private async Task<string> CallGeminiAsync(string prompt, CancellationToken cancellationToken)
    {
        var url = $"{_settings.BaseUrl}/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.2,
                responseMimeType = "application/json"
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(url, requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Gemini API returned {(int)response.StatusCode}: {error[..Math.Min(300, error.Length)]}");
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()
            ?? throw new InvalidOperationException("Gemini response text was null.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string BuildCodeSummary(IReadOnlyList<ExtractedFileInfo> files)
    {
        // Truncate to avoid hitting Gemini context limits (~32k tokens safe target)
        const int maxFiles = 200;
        var sb = new StringBuilder();
        foreach (var file in files.Take(maxFiles))
        {
            sb.AppendLine($"File: {file.FilePath}");
            sb.AppendLine($"  Namespace: {file.Namespace}");
            if (file.ClassNames.Count > 0)
                sb.AppendLine($"  Classes: {string.Join(", ", file.ClassNames)}");
            if (file.InterfaceNames.Count > 0)
                sb.AppendLine($"  Interfaces: {string.Join(", ", file.InterfaceNames)}");
        }
        if (files.Count > maxFiles)
            sb.AppendLine($"... and {files.Count - maxFiles} more files (truncated)");
        return sb.ToString();
    }

    private static C4Model BuildEmptyModel() => new(
        new C4ContextDiagram("Unknown System", "No code analysed", [], [], []),
        new C4ContainerDiagram([], []),
        new C4ComponentDiagram([], []),
        new C4ClassDiagram([], [])
    );
}
