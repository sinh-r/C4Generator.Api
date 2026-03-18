using C4Generator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C4Generator.Infrastructure.Persistence.Configurations;

internal sealed class InsightConfiguration : IEntityTypeConfiguration<Insight>
{
    public void Configure(EntityTypeBuilder<Insight> builder)
    {
        builder.ToTable("insights");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.ArchitectureModelId).HasColumnName("architecture_model_id");
        builder.Property(i => i.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
        builder.Property(i => i.Title).HasColumnName("title").HasMaxLength(300).IsRequired();
        builder.Property(i => i.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(i => i.Severity).HasColumnName("severity").HasMaxLength(50).IsRequired();
        builder.Property(i => i.DiscoveredAt).HasColumnName("discovered_at");

        builder.HasOne(i => i.ArchitectureModel)
            .WithMany()
            .HasForeignKey(i => i.ArchitectureModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
