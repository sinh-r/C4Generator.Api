using C4Generator.Domain.Enums;

namespace C4Generator.Application.Abstractions;

public record SourceRepositoryInfo(
    string ExternalId,
    string Name,
    string Owner,
    string Url,
    string? Description,
    string? DefaultBranch,
    string? Language
);

public interface ISourceControlProvider
{
    SourceControlProvider ProviderType { get; }

    Task<IReadOnlyList<SourceRepositoryInfo>> GetOrganizationRepositoriesAsync(
        string organization,
        string token,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SourceRepositoryInfo>> GetUserRepositoriesAsync(
        string username,
        string token,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SourceRepositoryInfo?> GetRepositoryAsync(
        string owner,
        string name,
        string token,
        CancellationToken cancellationToken = default);
}
