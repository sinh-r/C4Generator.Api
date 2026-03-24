using C4Generator.Application.Abstractions;
using C4Generator.Domain.Enums;
using Microsoft.Extensions.Logging;
using Octokit;

namespace C4Generator.Infrastructure.SourceControl.GitHub;

internal sealed class GitHubSourceControlProvider : ISourceControlProvider
{
    private readonly ILogger<GitHubSourceControlProvider> _logger;
    private const string ProductName = "C4Generator";

    public SourceControlProvider ProviderType => SourceControlProvider.GitHub;

    public GitHubSourceControlProvider(ILogger<GitHubSourceControlProvider> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<SourceRepositoryInfo>> GetOrganizationRepositoriesAsync(
        string organization,
        string token,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(token);
        try
        {
            var apiOptions = new ApiOptions { PageCount = 1, PageSize = pageSize, StartPage = page };
            var repos = await client.Repository.GetAllForOrg(organization, apiOptions);
            return repos.Select(MapToInfo).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch repositories for GitHub organization {Organization}", organization);
            throw;
        }
    }

    public async Task<IReadOnlyList<SourceRepositoryInfo>> GetUserRepositoriesAsync(
        string username,
        string token,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(token);
        try
        {
            var apiOptions = new ApiOptions { PageCount = 1, PageSize = pageSize, StartPage = page };
            var request = new RepositoryRequest { Type = RepositoryType.Owner };
            var repos = await client.Repository.GetAllForCurrent(request, apiOptions);
            return repos.Select(MapToInfo).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch repositories for GitHub user {Username}", username);
            throw;
        }
    }

    public async Task<SourceRepositoryInfo?> GetRepositoryAsync(
        string owner,
        string name,
        string token,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient(token);
        try
        {
            var repo = await client.Repository.Get(owner, name);
            return repo is null ? null : MapToInfo(repo);
        }
        catch (NotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch GitHub repository {Owner}/{Name}", owner, name);
            throw;
        }
    }

    private static GitHubClient CreateClient(string token)
    {
        var client = new GitHubClient(new ProductHeaderValue(ProductName));
        if (!string.IsNullOrWhiteSpace(token))
            client.Credentials = new Credentials(token);
        return client;
    }

    private static SourceRepositoryInfo MapToInfo(Repository repo) =>
        new(
            repo.Id.ToString(),
            repo.Name,
            repo.Owner.Login,
            repo.HtmlUrl,
            repo.Description,
            repo.DefaultBranch,
            repo.Language);
}
