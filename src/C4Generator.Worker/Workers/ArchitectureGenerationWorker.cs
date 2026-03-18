using C4Generator.Application.Abstractions;
using C4Generator.Application.Messages;
using C4Generator.Worker.Pipeline;

namespace C4Generator.Worker.Workers;

/// <summary>
/// Hosted service that continuously consumes GenerateArchitectureMessage from Kafka
/// and runs each message through the ArchitecturePipelineOrchestrator.
/// </summary>
public sealed class ArchitectureGenerationWorker : BackgroundService
{
    private readonly IKafkaConsumer<GenerateArchitectureMessage> _consumer;
    private readonly ArchitecturePipelineOrchestrator _orchestrator;
    private readonly ILogger<ArchitectureGenerationWorker> _logger;

    public ArchitectureGenerationWorker(
        IKafkaConsumer<GenerateArchitectureMessage> consumer,
        ArchitecturePipelineOrchestrator orchestrator,
        ILogger<ArchitectureGenerationWorker> logger)
    {
        _consumer = consumer;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ArchitectureGenerationWorker started. Waiting for messages...");

        await _consumer.StartAsync(async (message, ct) =>
        {
            _logger.LogInformation(
                "Received GenerateArchitectureMessage: RepositoryId={RepoId} ModelId={ModelId} JobId={JobId}",
                message.RepositoryId, message.ArchitectureModelId, message.JobId);

            var context = new ArchitectureGenerationContext
            {
                RepositoryId        = message.RepositoryId,
                ArchitectureModelId = message.ArchitectureModelId,
                JobId               = message.JobId,
                RepositoryUrl       = message.RepositoryUrl,
                Branch              = message.Branch
            };

            await _orchestrator.RunAsync(context, ct);

        }, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ArchitectureGenerationWorker stopping.");
        _consumer.Stop();
        await base.StopAsync(cancellationToken);
    }
}
