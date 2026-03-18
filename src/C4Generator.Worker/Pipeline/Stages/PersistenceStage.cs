using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using System.Text.Json;

namespace C4Generator.Worker.Pipeline.Stages;

/// <summary>
/// Stage 5 (final): Serialises the C4 model to JSON, persists Insights,
/// marks the ArchitectureModel as Generated and the Job as Completed.
/// </summary>
public sealed class PersistenceStage : IArchitecturePipelineStage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PersistenceStage> _logger;

    public PersistenceStage(IServiceScopeFactory scopeFactory, ILogger<PersistenceStage> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        if (context.C4ModelResult is null)
            throw new InvalidOperationException("C4ModelResult is null. AIInferenceStage must run first.");

        var modelJson = JsonSerializer.Serialize(context.C4ModelResult, JsonOptions);

        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var model = await unitOfWork.Architectures.GetByIdAsync(context.ArchitectureModelId, cancellationToken)
            ?? throw new InvalidOperationException($"ArchitectureModel {context.ArchitectureModelId} not found.");

        var job = await unitOfWork.Jobs.GetByIdAsync(context.JobId, cancellationToken)
            ?? throw new InvalidOperationException($"Job {context.JobId} not found.");

        // Persist insights
        if (context.Insights.Count > 0)
        {
            var insights = context.Insights.Select(i =>
                Insight.Create(context.ArchitectureModelId, i.Category, i.Title, i.Description, i.Severity));
            await unitOfWork.Insights.AddRangeAsync(insights, cancellationToken);
        }

        model.MarkAsGenerated(modelJson);
        job.MarkAsCompleted();

        unitOfWork.Architectures.Update(model);
        unitOfWork.Jobs.Update(job);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Persisted C4 model ({Bytes} bytes) and {InsightCount} insights for Model={ModelId}",
            modelJson.Length, context.Insights.Count, context.ArchitectureModelId);
    }
}
