using C4Generator.Application.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace C4Generator.Api.HealthChecks;

public sealed class GitHubHealthCheck : IHealthCheck
{
    private readonly IGitHubService _gitHubService;

    public GitHubHealthCheck(IGitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isReachable = await _gitHubService.IsReachableAsync(cancellationToken);
        return isReachable
            ? HealthCheckResult.Healthy("GitHub API is reachable.")
            : HealthCheckResult.Degraded("GitHub API is unreachable or rate-limited.");
    }
}
