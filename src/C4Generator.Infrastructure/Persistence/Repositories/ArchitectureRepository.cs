using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace C4Generator.Infrastructure.Persistence.Repositories;

internal sealed class ArchitectureRepository : IArchitectureRepository
{
    private readonly AppDbContext _context;

    public ArchitectureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ArchitectureModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.ArchitectureModels.FindAsync([id], cancellationToken);

    public async Task<ArchitectureModel?> GetByRepositoryIdAsync(Guid repositoryId, CancellationToken cancellationToken = default)
        => await _context.ArchitectureModels
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.RepositoryId == repositoryId, cancellationToken);

    public async Task AddAsync(ArchitectureModel model, CancellationToken cancellationToken = default)
        => await _context.ArchitectureModels.AddAsync(model, cancellationToken);

    public void Update(ArchitectureModel model)
        => _context.ArchitectureModels.Update(model);
}
