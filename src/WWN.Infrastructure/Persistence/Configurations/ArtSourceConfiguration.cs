using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class ArtSourceConfiguration : IEntityTypeConfiguration<ArtSource>
{
    public void Configure(EntityTypeBuilder<ArtSource> builder)
    {
        builder.ToTable("ArtSources");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SortOrder).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        // Stable seed rows; IDs are explicit so FKs in Arts never break.
        builder.HasData(
            new { Id = 1, Code = "Mage", DisplayName = "Mage", Description = (string?)null, SortOrder = 1 },
            new { Id = 2, Code = "PartialMage", DisplayName = "Partial Mage", Description = (string?)null, SortOrder = 2 }
        );
    }
}
