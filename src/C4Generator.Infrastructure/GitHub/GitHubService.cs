using C4Generator.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;

namespace C4Generator.Infrastructure.GitHub;

internal sealed class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(IOptions<GitHubSettings> settings, ILogger<GitHubService> logger)
    {
        _logger = logger;
        _client = new GitHubClient(new ProductHeaderValue(settings.Value.AppName));

        if (!string.IsNullOrWhiteSpace(settings.Value.Token))
            _client.Credentials = new Credentials(settings.Value.Token);
    }

    public async Task<IReadOnlyList<GitHubRepositoryInfo>> GetOrganizationRepositoriesAsync(string organization, CancellationToken cancellationToken = default)
    {
        try
        {
            var repos = await _client.Repository.GetAllForOrg(organization);
            return repos.Select(MapToInfo).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch repositories for organization {Organization}", organization);
            throw;
        }
    }

    public async Task<GitHubRepositoryInfo?> GetRepositoryAsync(string owner, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var repo = await _client.Repository.Get(owner, name);
            return repo is null ? null : MapToInfo(repo);
        }
        catch (NotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch repository {Owner}/{Name}", owner, name);
            throw;
        }
    }

    public async Task<bool> IsReachableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.Miscellaneous.GetAllGitIgnoreTemplates();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static GitHubRepositoryInfo MapToInfo(Octokit.Repository repo) =>
        new(repo.Name, repo.Owner.Login, repo.HtmlUrl, repo.Description, repo.DefaultBranch, repo.Language);
}
