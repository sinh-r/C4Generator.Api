using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using C4Generator.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace C4Generator.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepositoryRepository Repositories { get; }
    public IArchitectureRepository Architectures { get; }
    public IJobRepository Jobs { get; }
    public IInsightRepository Insights { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Repositories = new RepositoryRepository(context);
        Architectures = new ArchitectureRepository(context);
        Jobs = new JobRepository(context);
        Insights = new InsightRepository(context);
        Users = new UserRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new ConflictException("A record with the same unique value already exists.");
        }
    }
}
