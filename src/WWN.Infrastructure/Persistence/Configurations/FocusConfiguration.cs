using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class FocusConfiguration : IEntityTypeConfiguration<Focus>
{
    public void Configure(EntityTypeBuilder<Focus> builder)
    {
        builder.ToTable("Foci");
        builder.HasKey(focus => focus.Id);
        builder.Property(focus => focus.Id).ValueGeneratedNever();
        builder.Property(focus => focus.Name).HasMaxLength(100).IsRequired();
        builder.Property(focus => focus.ConditionalActive).HasDefaultValue(false);
        builder.OwnsMany(focus => focus.Effects, e =>
        {
            e.ToJson();
        });
    }
}
