using C4Generator.Application.Abstractions;
using C4Generator.Application.Commands.Repositories;
using C4Generator.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace C4Generator.Api.BackgroundJobs;

public sealed class RepositorySyncSettings
{
    public string Organization { get; set; } = string.Empty;
    public int IntervalMinutes { get; set; } = 60;
}

public sealed class RepositorySyncJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RepositorySyncJob> _logger;
    private readonly RepositorySyncSettings _settings;

    public RepositorySyncJob(
        IServiceScopeFactory scopeFactory,
        ILogger<RepositorySyncJob> logger,
        IOptions<RepositorySyncSettings> settings)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RepositorySyncJob started. Organization: {Org}", _settings.Organization);

        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncRepositoriesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(_settings.IntervalMinutes), stoppingToken);
        }
    }

    private async Task SyncRepositoriesAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.Organization))
        {
            _logger.LogDebug("RepositorySyncJob: No organization configured, skipping sync.");
            return;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var gitHubService = scope.ServiceProvider.GetRequiredService<IGitHubService>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var remoteRepos = await gitHubService.GetOrganizationRepositoriesAsync(_settings.Organization, cancellationToken);
            var existingRepos = await unitOfWork.Repositories.GetAllAsync(cancellationToken);
            var existingUrls = existingRepos.Select(r => r.Url).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var remote in remoteRepos)
            {
                if (!existingUrls.Contains(remote.Url))
                {
                    _logger.LogInformation("Syncing new repository: {Owner}/{Name}", remote.Owner, remote.Name);
                    await mediator.Send(new CreateRepositoryCommand(remote.Name, remote.Owner, remote.Url, remote.Description), cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RepositorySyncJob: Error syncing repositories for {Org}", _settings.Organization);
        }
    }
}
