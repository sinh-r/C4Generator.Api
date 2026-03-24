using C4Generator.Domain.Entities;
using C4Generator.Domain.Enums;
using C4Generator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace C4Generator.Infrastructure.Persistence.Repositories;

internal sealed class RepositoryRepository : IRepositoryRepository
{
    private readonly AppDbContext _context;

    public RepositoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Repository?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Repositories.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Repository>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Repositories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Repository>> GetByProviderAndOwnerAsync(
        SourceControlProvider provider,
        string ownerOrOrg,
        CancellationToken cancellationToken = default)
        => await _context.Repositories
            .AsNoTracking()
            .Where(r => r.Provider == provider && r.Owner == ownerOrOrg)
            .ToListAsync(cancellationToken);

    public async Task<Repository?> GetByExternalIdAsync(
        SourceControlProvider provider,
        string externalId,
        CancellationToken cancellationToken = default)
        => await _context.Repositories
            .FirstOrDefaultAsync(r => r.Provider == provider && r.ExternalId == externalId, cancellationToken);

    public async Task AddAsync(Repository repository, CancellationToken cancellationToken = default)
        => await _context.Repositories.AddAsync(repository, cancellationToken);

    public void Update(Repository repository)
        => _context.Repositories.Update(repository);

    public void Delete(Repository repository)
        => _context.Repositories.Remove(repository);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Repositories.AnyAsync(r => r.Id == id, cancellationToken);
}
