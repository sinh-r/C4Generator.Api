using C4Generator.Domain.Enums;
using C4Generator.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace C4Generator.Api.BackgroundJobs;

public sealed class ArchitectureCleanupSettings
{
    public int RetentionDays { get; set; } = 30;
    public int IntervalHours { get; set; } = 24;
}

public sealed class ArchitectureCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ArchitectureCleanupJob> _logger;
    private readonly ArchitectureCleanupSettings _settings;

    public ArchitectureCleanupJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ArchitectureCleanupJob> logger,
        IOptions<ArchitectureCleanupSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ArchitectureCleanupJob started. RetentionDays: {Days}", _settings.RetentionDays);

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(_settings.IntervalHours), stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("ArchitectureCleanupJob: Cleanup cycle started.");
            // Concrete cleanup logic (e.g., deleting failed/stale models) is implemented
            // after the persistence query layer is extended with date-based filtering.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ArchitectureCleanupJob: Error during cleanup.");
        }
    }
}
