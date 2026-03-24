using C4Generator.Application.Abstractions;
using C4Generator.Domain.Enums;

namespace C4Generator.Infrastructure.SourceControl.GitLab;

internal sealed class GitLabSourceControlProvider : ISourceControlProvider
{
    public SourceControlProvider ProviderType => SourceControlProvider.GitLab;

    public Task<IReadOnlyList<SourceRepositoryInfo>> GetOrganizationRepositoriesAsync(
        string organization, string token, int page, int pageSize, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("GitLab provider is not yet supported.");

    public Task<IReadOnlyList<SourceRepositoryInfo>> GetUserRepositoriesAsync(
        string username, string token, int page, int pageSize, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("GitLab provider is not yet supported.");

    public Task<SourceRepositoryInfo?> GetRepositoryAsync(
        string owner, string name, string token, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("GitLab provider is not yet supported.");
}
