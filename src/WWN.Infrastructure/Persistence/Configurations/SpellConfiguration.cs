using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class SpellConfiguration : IEntityTypeConfiguration<Spell>
{
    public void Configure(EntityTypeBuilder<Spell> builder)
    {
        builder.ToTable("Spells");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.SpellLevel).IsRequired();
        builder.Property(s => s.Description).IsRequired();
        builder.Property(s => s.Summary).HasMaxLength(500);
    }
}
