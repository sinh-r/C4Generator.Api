using C4Generator.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace C4Generator.Worker.Pipeline;

public sealed class ArchitecturePipelineOrchestrator
{
    private readonly IEnumerable<IArchitecturePipelineStage> _stages;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ArchitecturePipelineOrchestrator> _logger;

    public ArchitecturePipelineOrchestrator(
        IEnumerable<IArchitecturePipelineStage> stages,
        IServiceScopeFactory scopeFactory,
        ILogger<ArchitecturePipelineOrchestrator> logger)
    {
        _stages = stages;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task RunAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Pipeline starting for ArchitectureModel={ModelId} Repository={RepoId}",
            context.ArchitectureModelId, context.RepositoryId);

        foreach (var stage in _stages)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var stageName = stage.GetType().Name;
            try
            {
                _logger.LogDebug("Executing stage: {Stage}", stageName);
                await stage.ExecuteAsync(context, cancellationToken);
                _logger.LogDebug("Stage completed: {Stage}", stageName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Pipeline cancelled at stage {Stage}", stageName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pipeline failed at stage {Stage} for Model={ModelId}", stageName, context.ArchitectureModelId);
                await MarkFailedAsync(context, ex.Message, cancellationToken);
                return;
            }
        }

        _logger.LogInformation(
            "Pipeline completed successfully for ArchitectureModel={ModelId}",
            context.ArchitectureModelId);
    }

    private async Task MarkFailedAsync(ArchitectureGenerationContext context, string error, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var model = await unitOfWork.Architectures.GetByIdAsync(context.ArchitectureModelId, cancellationToken);
            if (model is not null)
            {
                model.MarkAsFailed(error);
                unitOfWork.Architectures.Update(model);
            }

            var job = await unitOfWork.Jobs.GetByIdAsync(context.JobId, cancellationToken);
            if (job is not null)
            {
                job.MarkAsFailed(error);
                unitOfWork.Jobs.Update(job);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist failure state for Model={ModelId}", context.ArchitectureModelId);
        }
    }
}
