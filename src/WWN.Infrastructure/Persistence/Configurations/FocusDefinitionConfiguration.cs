using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class FocusDefinitionConfiguration : IEntityTypeConfiguration<FocusDefinition>
{
    public void Configure(EntityTypeBuilder<FocusDefinition> builder)
    {
        builder.ToTable("FocusDefinitions");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).HasMaxLength(100).IsRequired();
        builder.Property(f => f.Description).HasMaxLength(1000);
        builder.Property(f => f.Level1Description).IsRequired();
        builder.Property(f => f.Level2Description);
        builder.Property(f => f.HasLevel2).IsRequired();
        builder.Property(f => f.CanTakeMultipleTimes).IsRequired();
    }
}
