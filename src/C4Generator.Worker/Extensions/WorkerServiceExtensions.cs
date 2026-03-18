using C4Generator.Application.Extensions;
using C4Generator.Infrastructure.Extensions;
using C4Generator.Worker.Pipeline;
using C4Generator.Worker.Pipeline.Stages;
using C4Generator.Worker.Workers;

namespace C4Generator.Worker.Extensions;

public static class WorkerServiceExtensions
{
    public static IServiceCollection AddWorkerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Shared layers
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        // Worker config
        services.Configure<WorkerSettings>(configuration.GetSection("Worker"));
        services.Configure<GeminiSettings>(configuration.GetSection("Gemini"));

        // HttpClient for Gemini (typed client — must forward-resolve via interface to use the configured HttpClient)
        services.AddHttpClient<AIInferenceStage>();

        // NOTE: IKafkaConsumer<T> open generic is registered by AddInfrastructureServices above.

        // Pipeline stages — ORDER IS THE EXECUTION ORDER
        services.AddTransient<IArchitecturePipelineStage, StatusUpdateStage>();
        services.AddTransient<IArchitecturePipelineStage, RepositoryCloneStage>();
        services.AddTransient<IArchitecturePipelineStage, CodeAnalysisStage>();
        // Forward-resolve so the typed HttpClient registered above is used
        services.AddTransient<IArchitecturePipelineStage>(sp => sp.GetRequiredService<AIInferenceStage>());
        services.AddTransient<IArchitecturePipelineStage, PersistenceStage>();

        // Orchestrator
        services.AddTransient<ArchitecturePipelineOrchestrator>();

        // Hosted worker
        services.AddHostedService<ArchitectureGenerationWorker>();

        return services;
    }
}
