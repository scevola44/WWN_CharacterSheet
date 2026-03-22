using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class KnownSpellConfiguration : IEntityTypeConfiguration<KnownSpell>
{
    public void Configure(EntityTypeBuilder<KnownSpell> builder)
    {
        builder.ToTable("KnownSpells");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Id).ValueGeneratedNever();
        builder.HasOne(k => k.Spell)
               .WithMany()
               .HasForeignKey(k => k.SpellId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
