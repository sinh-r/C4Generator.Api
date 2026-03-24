using C4Generator.Application.Abstractions;
using C4Generator.Domain.Interfaces;
using C4Generator.Infrastructure.Auth;
using C4Generator.Infrastructure.GitHub;
using C4Generator.Infrastructure.Messaging;
using C4Generator.Infrastructure.Persistence;
using C4Generator.Infrastructure.SourceControl;
using C4Generator.Infrastructure.SourceControl.AwsCodeCommit;
using C4Generator.Infrastructure.SourceControl.AzureAdo;
using C4Generator.Infrastructure.SourceControl.Bitbucket;
using C4Generator.Infrastructure.SourceControl.GitHub;
using C4Generator.Infrastructure.SourceControl.GitLab;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace C4Generator.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Messaging (Confluent Kafka)
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<IQueuePublisher, KafkaQueuePublisher>();
        services.AddSingleton(typeof(IKafkaConsumer<>), typeof(KafkaConsumer<>));

        // GitHub (legacy service, kept for health check compatibility)
        services.Configure<GitHubSettings>(configuration.GetSection("GitHub"));
        services.AddScoped<IGitHubService, GitHubService>();

        // Source control providers
        services.AddScoped<ISourceControlProvider, GitHubSourceControlProvider>();
        services.AddScoped<ISourceControlProvider, GitLabSourceControlProvider>();
        services.AddScoped<ISourceControlProvider, BitbucketSourceControlProvider>();
        services.AddScoped<ISourceControlProvider, AzureAdoSourceControlProvider>();
        services.AddScoped<ISourceControlProvider, AwsCodeCommitSourceControlProvider>();
        services.AddScoped<ISourceControlProviderFactory, SourceControlProviderFactory>();

        // Auth
        services.AddOptions<JwtSettings>()
            .BindConfiguration("Auth")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
