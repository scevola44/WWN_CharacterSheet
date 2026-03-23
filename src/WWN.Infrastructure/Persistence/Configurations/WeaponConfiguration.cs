using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        builder.OwnsOne(weapon => weapon.DamageDie, d =>
        {
            d.Property(damageDie => damageDie.Count).HasColumnName("DamageDieCount");
            d.Property(damageDie => damageDie.Sides).HasColumnName("DamageDieSides");
        });
        builder.OwnsOne(weapon => weapon.Shock, s =>
        {
            s.Property(damageDie => damageDie.Damage).HasColumnName("ShockDamage");
            s.Property(damageDie => damageDie.AcThreshold).HasColumnName("ShockAcThreshold");
        });
        builder.Property(weapon => weapon.Tags).HasConversion<int>();
        builder.Property(weapon => weapon.AttributeModifier).HasConversion<string>().HasMaxLength(20);
    }
}
