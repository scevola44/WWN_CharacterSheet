using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class ArtConfiguration : IEntityTypeConfiguration<Art>
{
    public void Configure(EntityTypeBuilder<Art> builder)
    {
        builder.ToTable("Arts");
        builder.HasKey(art => art.Id);
        builder.Property(art => art.Name).HasMaxLength(200).IsRequired();
        builder.Property(art => art.Description).IsRequired();
        builder.Property(art => art.Summary).HasMaxLength(500);
        builder.Property(art => art.MinLevel).IsRequired();
        builder.Property(art => art.Source).HasMaxLength(50).IsRequired();
        builder.Property(art => art.EffortCost).HasConversion<string>().HasMaxLength(20);
    }
}
