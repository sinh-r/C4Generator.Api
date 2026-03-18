using C4Generator.Api.BackgroundJobs;
using C4Generator.Api.Extensions;
using C4Generator.Api.Logging;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/c4generator-.log", rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {CorrelationId} {UserId} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting C4Generator.Api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // DI registrations
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApiServices(builder.Configuration);

    // Background jobs
    builder.Services.Configure<RepositorySyncSettings>(builder.Configuration.GetSection("RepositorySync"));
    builder.Services.Configure<ArchitectureCleanupSettings>(builder.Configuration.GetSection("ArchitectureCleanup"));
    builder.Services.AddHostedService<RepositorySyncJob>();
    builder.Services.AddHostedService<ArchitectureCleanupJob>();

    // Logging helpers
    builder.Services.AddSingleton<LoggingEnricher>();
    builder.Services.AddSingleton<RequestLogger>();

    var app = builder.Build();

    // Middleware pipeline
    app.UseApiMiddleware();
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoint registrations
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.MapControllers();

    // Health check endpoints
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready");
    app.MapHealthChecks("/health/live");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "C4Generator.Api terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

