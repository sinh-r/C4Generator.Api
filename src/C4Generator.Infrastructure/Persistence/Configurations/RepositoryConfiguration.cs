using C4Generator.Domain.Entities;
using C4Generator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4Generator.Infrastructure.Persistence.Configurations;

internal sealed class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        builder.ToTable("repositories");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Owner)
            .HasColumnName("owner")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Url)
            .HasColumnName("url")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(r => r.DefaultBranch)
            .HasColumnName("default_branch")
            .HasMaxLength(200);

        builder.Property(r => r.Language)
            .HasColumnName("language")
            .HasMaxLength(100);

        builder.Property(r => r.ArchitectureStatus)
            .HasColumnName("architecture_status")
            .HasConversion<int>()
            .HasDefaultValue(ArchitectureStatus.NotGenerated);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(r => r.Url).IsUnique();
    }
}
