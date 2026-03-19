using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class ArmorConfiguration : IEntityTypeConfiguration<Armor>
{
    public void Configure(EntityTypeBuilder<Armor> builder)
    {
        builder.Property(a => a.AcBonus).HasColumnName("AcBonus");
        builder.Property(a => a.IsShield).HasColumnName("IsShield");
    }
}
