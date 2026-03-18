namespace C4Generator.Worker.Pipeline;

public interface IArchitecturePipelineStage
{
    Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken);
}
