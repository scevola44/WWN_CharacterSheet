using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Aggregates;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("Characters");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Background).HasMaxLength(200);
        builder.Property(c => c.Origin).HasMaxLength(200);
        builder.Property(c => c.Class).HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.PartialClassA).HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.PartialClassB).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(c => c.Attributes)
               .WithOne()
               .HasForeignKey(a => a.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Skills)
               .WithOne()
               .HasForeignKey(s => s.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Foci)
               .WithOne()
               .HasForeignKey(f => f.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Inventory)
               .WithOne()
               .HasForeignKey(i => i.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Spellbook)
               .WithOne()
               .HasForeignKey(k => k.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.SpellSlotsUsed)
               .HasConversion(
                   v => string.Join(',', v),
                   v => v.Split(',').Select(int.Parse).ToArray())
               .HasColumnType("TEXT");

        builder.Navigation(c => c.Attributes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Skills).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Foci).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Inventory).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Spellbook).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
