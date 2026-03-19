using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        builder.OwnsOne(w => w.DamageDie, d =>
        {
            d.Property(x => x.Count).HasColumnName("DamageDieCount");
            d.Property(x => x.Sides).HasColumnName("DamageDieSides");
        });
        builder.OwnsOne(w => w.Shock, s =>
        {
            s.Property(x => x.Damage).HasColumnName("ShockDamage");
            s.Property(x => x.AcThreshold).HasColumnName("ShockAcThreshold");
        });
        builder.Property(w => w.Tags).HasConversion<int>();
        builder.Property(w => w.AttributeModifier).HasConversion<string>().HasMaxLength(20);
    }
}
