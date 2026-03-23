using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class KnownSpellConfiguration : IEntityTypeConfiguration<KnownSpell>
{
    public void Configure(EntityTypeBuilder<KnownSpell> builder)
    {
        builder.ToTable("KnownSpells");
        builder.HasKey(knownSpell => knownSpell.Id);
        builder.Property(knownSpell => knownSpell.Id).ValueGeneratedNever();
        builder.HasOne(knownSpell => knownSpell.Spell)
               .WithMany()
               .HasForeignKey(knownSpell => knownSpell.SpellId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
