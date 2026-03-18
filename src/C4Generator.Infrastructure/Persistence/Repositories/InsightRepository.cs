using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace C4Generator.Infrastructure.Persistence.Repositories;

internal sealed class InsightRepository : IInsightRepository
{
    private readonly AppDbContext _context;

    public InsightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Insight>> GetByArchitectureModelIdAsync(Guid architectureModelId, CancellationToken cancellationToken = default)
        => await _context.Insights
            .AsNoTracking()
            .Where(i => i.ArchitectureModelId == architectureModelId)
            .ToListAsync(cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Insight> insights, CancellationToken cancellationToken = default)
        => await _context.Insights.AddRangeAsync(insights, cancellationToken);
}
