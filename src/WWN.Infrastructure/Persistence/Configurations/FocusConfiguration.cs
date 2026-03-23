using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class FocusConfiguration : IEntityTypeConfiguration<Focus>
{
    public void Configure(EntityTypeBuilder<Focus> builder)
    {
        builder.ToTable("Foci");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();
        builder.Property(f => f.Name).HasMaxLength(100).IsRequired();
        builder.OwnsMany(f => f.Effects, e =>
        {
            e.ToJson();
        });
    }
}
