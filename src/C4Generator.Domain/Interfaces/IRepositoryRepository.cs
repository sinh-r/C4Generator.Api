using C4Generator.Domain.Entities;

namespace C4Generator.Domain.Interfaces;

public interface IRepositoryRepository
{
    Task<Repository?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Repository>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Repository repository, CancellationToken cancellationToken = default);
    void Delete(Repository repository);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
