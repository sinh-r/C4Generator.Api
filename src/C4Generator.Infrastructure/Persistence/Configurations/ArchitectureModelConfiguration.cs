using C4Generator.Domain.Entities;
using C4Generator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4Generator.Infrastructure.Persistence.Configurations;

internal sealed class ArchitectureModelConfiguration : IEntityTypeConfiguration<ArchitectureModel>
{
    public void Configure(EntityTypeBuilder<ArchitectureModel> builder)
    {
        builder.ToTable("architecture_models");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.RepositoryId).HasColumnName("repository_id");
        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .HasDefaultValue(ArchitectureStatus.Pending);

        builder.Property(a => a.ModelJson).HasColumnName("model_json");
        builder.Property(a => a.ErrorMessage).HasColumnName("error_message").HasMaxLength(2000);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(a => a.Repository)
            .WithMany()
            .HasForeignKey(a => a.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
