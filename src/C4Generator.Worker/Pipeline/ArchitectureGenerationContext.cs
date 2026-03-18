using C4Generator.Domain.Models;

namespace C4Generator.Worker.Pipeline;

/// <summary>
/// Shared mutable context passed through all pipeline stages.
/// Each stage reads/writes its section; later stages see earlier-stage results.
/// </summary>
public sealed class ArchitectureGenerationContext
{
    // ── Populated from the Kafka message before pipeline starts ──────────────

    public Guid RepositoryId { get; init; }
    public Guid ArchitectureModelId { get; init; }
    public Guid JobId { get; init; }
    public string RepositoryUrl { get; init; } = string.Empty;
    public string Branch { get; init; } = "main";

    // ── Set by RepositoryCloneStage ──────────────────────────────────────────

    public string LocalRepoPath { get; set; } = string.Empty;

    // ── Set by CodeAnalysisStage ─────────────────────────────────────────────

    public IReadOnlyList<ExtractedFileInfo> ExtractedFiles { get; set; } = [];

    // ── Set by AIInferenceStage ──────────────────────────────────────────────

    public C4Model? C4ModelResult { get; set; }
    public IReadOnlyList<ExtractedInsight> Insights { get; set; } = [];
}

/// <summary>Roslyn-extracted structural information from a single source file.</summary>
public sealed record ExtractedFileInfo(
    string FilePath,
    string Namespace,
    IReadOnlyList<string> ClassNames,
    IReadOnlyList<string> InterfaceNames
);

/// <summary>An architectural insight produced by AI inference.</summary>
public sealed record ExtractedInsight(
    string Category,
    string Title,
    string Description,
    string Severity    // "Low" | "Medium" | "High" | "Critical"
);
