using C4Generator.Application.Abstractions;
using C4Generator.Domain.Interfaces;
using C4Generator.Infrastructure.Auth;
using C4Generator.Infrastructure.GitHub;
using C4Generator.Infrastructure.Messaging;
using C4Generator.Infrastructure.Persistence;
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

        // GitHub
        services.Configure<GitHubSettings>(configuration.GetSection("GitHub"));
        services.AddScoped<IGitHubService, GitHubService>();

        // Auth
        services.Configure<JwtSettings>(configuration.GetSection("Auth"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
