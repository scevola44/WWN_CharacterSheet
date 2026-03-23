using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class SpellConfiguration : IEntityTypeConfiguration<Spell>
{
    public void Configure(EntityTypeBuilder<Spell> builder)
    {
        builder.ToTable("Spells");
        builder.HasKey(spell => spell.Id);
        builder.Property(spell => spell.Name).HasMaxLength(200).IsRequired();
        builder.Property(spell => spell.SpellLevel).IsRequired();
        builder.Property(spell => spell.Description).IsRequired();
        builder.Property(spell => spell.Summary).HasMaxLength(500);
    }
}
