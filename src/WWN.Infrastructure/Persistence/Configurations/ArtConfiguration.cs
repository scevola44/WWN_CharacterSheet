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
        builder.Property(art => art.SourceId).IsRequired();
        builder.Property(art => art.EffortCost).IsRequired();
        builder.HasOne(art => art.SourceNavigation)
            .WithMany()
            .HasForeignKey(art => art.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(art => art.OwnerId).HasMaxLength(450);
        builder.HasIndex(art => art.OwnerId);
    }
}
