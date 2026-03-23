using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class FocusDefinitionConfiguration : IEntityTypeConfiguration<FocusDefinition>
{
    public void Configure(EntityTypeBuilder<FocusDefinition> builder)
    {
        builder.ToTable("FocusDefinitions");
        builder.HasKey(focus => focus.Id);
        builder.Property(focus => focus.Name).HasMaxLength(100).IsRequired();
        builder.Property(focus => focus.Description).HasMaxLength(1000);
        builder.Property(focus => focus.Level1Description).IsRequired();
        builder.Property(focus => focus.Level2Description);
        builder.Property(focus => focus.HasLevel2).IsRequired();
        builder.Property(focus => focus.CanTakeMultipleTimes).IsRequired();
        builder.OwnsMany(fd => fd.Level1Effects, e => { e.ToJson(); });
        builder.OwnsMany(fd => fd.Level2Effects, e => { e.ToJson(); });
    }
}
