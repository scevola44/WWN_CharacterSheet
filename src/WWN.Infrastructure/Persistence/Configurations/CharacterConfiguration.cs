using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Aggregates;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("Characters");
        builder.HasKey(character => character.Id);
        builder.Property(character => character.Name).HasMaxLength(200).IsRequired();
        builder.Property(character => character.Background).HasMaxLength(200);
        builder.Property(character => character.Origin).HasMaxLength(200);
        builder.Property(character => character.Class).HasConversion<string>().HasMaxLength(50);
        builder.Property(character => character.PartialClassA).HasConversion<string>().HasMaxLength(50);
        builder.Property(character => character.PartialClassB).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(character => character.Attributes)
               .WithOne()
               .HasForeignKey(a => a.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.Skills)
               .WithOne()
               .HasForeignKey(s => s.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.Foci)
               .WithOne()
               .HasForeignKey(f => f.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.Inventory)
               .WithOne()
               .HasForeignKey(i => i.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.Spellbook)
               .WithOne()
               .HasForeignKey(k => k.CharacterId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(character => character.SpellSlotsUsed)
               .HasConversion(
                   v => string.Join(',', v),
                   v => v.Split(',').Select(int.Parse).ToArray())
               .HasColumnType("TEXT");

        builder.Navigation(character => character.Attributes).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(character => character.Skills).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(character => character.Foci).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(character => character.Inventory).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(character => character.Spellbook).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
