using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace C4Generator.Infrastructure.Persistence.Repositories;

internal sealed class JobRepository : IJobRepository
{
    private readonly AppDbContext _context;

    public JobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Jobs.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Jobs.AsNoTracking().OrderByDescending(j => j.CreatedAt).ToListAsync(cancellationToken);

    public async Task AddAsync(Job job, CancellationToken cancellationToken = default)
        => await _context.Jobs.AddAsync(job, cancellationToken);

    public void Update(Job job)
        => _context.Jobs.Update(job);
}
