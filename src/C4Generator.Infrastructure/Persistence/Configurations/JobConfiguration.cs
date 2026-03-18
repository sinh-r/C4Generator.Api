using C4Generator.Domain.Entities;
using C4Generator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4Generator.Infrastructure.Persistence.Configurations;

internal sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id).HasColumnName("id");
        builder.Property(j => j.RepositoryId).HasColumnName("repository_id");
        builder.Property(j => j.JobType).HasColumnName("job_type").HasMaxLength(100).IsRequired();
        builder.Property(j => j.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(JobStatus.Pending);

        builder.Property(j => j.ErrorMessage).HasColumnName("error_message").HasMaxLength(2000);
        builder.Property(j => j.CreatedAt).HasColumnName("created_at");
        builder.Property(j => j.StartedAt).HasColumnName("started_at");
        builder.Property(j => j.CompletedAt).HasColumnName("completed_at");

        builder.HasOne(j => j.Repository)
            .WithMany()
            .HasForeignKey(j => j.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
