using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class KnownArtConfiguration : IEntityTypeConfiguration<KnownArt>
{
    public void Configure(EntityTypeBuilder<KnownArt> builder)
    {
        builder.ToTable("KnownArts");
        builder.HasKey(knownArt => knownArt.Id);
        builder.Property(knownArt => knownArt.Id).ValueGeneratedNever();
        builder.HasOne(knownArt => knownArt.Art)
               .WithMany()
               .HasForeignKey(knownArt => knownArt.ArtId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
