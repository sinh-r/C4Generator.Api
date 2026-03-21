namespace C4Generator.Domain.Interfaces;

public interface IUnitOfWork
{
    IRepositoryRepository Repositories { get; }
    IArchitectureRepository Architectures { get; }
    IJobRepository Jobs { get; }
    IInsightRepository Insights { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
