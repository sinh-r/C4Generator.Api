using C4Generator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace C4Generator.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<ArchitectureModel> ArchitectureModels => Set<ArchitectureModel>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Insight> Insights => Set<Insight>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
