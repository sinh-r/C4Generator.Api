using C4Generator.Domain.Entities;
using C4Generator.Domain.Enums;

namespace C4Generator.Domain.Interfaces;

public interface IRepositoryRepository
{
    Task<Repository?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Repository>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Repository>> GetByProviderAndOwnerAsync(SourceControlProvider provider, string ownerOrOrg, CancellationToken cancellationToken = default);
    Task<Repository?> GetByExternalIdAsync(SourceControlProvider provider, string externalId, CancellationToken cancellationToken = default);
    Task AddAsync(Repository repository, CancellationToken cancellationToken = default);
    void Update(Repository repository);
    void Delete(Repository repository);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
