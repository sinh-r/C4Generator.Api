using C4Generator.Application.Abstractions;
using C4Generator.Domain.Enums;

namespace C4Generator.Infrastructure.SourceControl.AzureAdo;

internal sealed class AzureAdoSourceControlProvider : ISourceControlProvider
{
    public SourceControlProvider ProviderType => SourceControlProvider.AzureAdo;

    public Task<IReadOnlyList<SourceRepositoryInfo>> GetOrganizationRepositoriesAsync(
        string organization, string token, int page, int pageSize, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Azure DevOps provider is not yet supported.");

    public Task<IReadOnlyList<SourceRepositoryInfo>> GetUserRepositoriesAsync(
        string username, string token, int page, int pageSize, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Azure DevOps provider is not yet supported.");

    public Task<SourceRepositoryInfo?> GetRepositoryAsync(
        string owner, string name, string token, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Azure DevOps provider is not yet supported.");
}
