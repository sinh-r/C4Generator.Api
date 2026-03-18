using C4Generator.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace C4Generator.Worker.Pipeline.Stages;

/// <summary>
/// Stage 1: Marks the Job as Running and the ArchitectureModel as Processing.
/// Must be the very first stage so subsequent failures are tracked from the start.
/// </summary>
public sealed class StatusUpdateStage : IArchitecturePipelineStage
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StatusUpdateStage> _logger;

    public StatusUpdateStage(IServiceScopeFactory scopeFactory, ILogger<StatusUpdateStage> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var model = await unitOfWork.Architectures.GetByIdAsync(context.ArchitectureModelId, cancellationToken)
            ?? throw new InvalidOperationException($"ArchitectureModel {context.ArchitectureModelId} not found.");

        var job = await unitOfWork.Jobs.GetByIdAsync(context.JobId, cancellationToken)
            ?? throw new InvalidOperationException($"Job {context.JobId} not found.");

        model.MarkAsProcessing();
        job.MarkAsRunning();

        unitOfWork.Architectures.Update(model);
        unitOfWork.Jobs.Update(job);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Status updated: Job={JobId} → Running, Model={ModelId} → Processing", context.JobId, context.ArchitectureModelId);
    }
}
