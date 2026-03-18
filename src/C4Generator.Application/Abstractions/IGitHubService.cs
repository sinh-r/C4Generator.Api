namespace C4Generator.Application.Abstractions;

public record GitHubRepositoryInfo(
    string Name,
    string Owner,
    string Url,
    string? Description,
    string? DefaultBranch,
    string? Language
);

public interface IGitHubService
{
    Task<IReadOnlyList<GitHubRepositoryInfo>> GetOrganizationRepositoriesAsync(string organization, CancellationToken cancellationToken = default);
    Task<GitHubRepositoryInfo?> GetRepositoryAsync(string owner, string name, CancellationToken cancellationToken = default);
    Task<bool> IsReachableAsync(CancellationToken cancellationToken = default);
}
