using C4Generator.Domain.Entities;

namespace C4Generator.Domain.Interfaces;

public interface IInsightRepository
{
    Task<IReadOnlyList<Insight>> GetByArchitectureModelIdAsync(Guid architectureModelId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Insight> insights, CancellationToken cancellationToken = default);
}
