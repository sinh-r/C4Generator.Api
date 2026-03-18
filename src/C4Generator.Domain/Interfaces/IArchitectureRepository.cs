using C4Generator.Domain.Entities;

namespace C4Generator.Domain.Interfaces;

public interface IArchitectureRepository
{
    Task<ArchitectureModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ArchitectureModel?> GetByRepositoryIdAsync(Guid repositoryId, CancellationToken cancellationToken = default);
    Task AddAsync(ArchitectureModel model, CancellationToken cancellationToken = default);
    void Update(ArchitectureModel model);
}
