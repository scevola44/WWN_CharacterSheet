using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class ClassAbilityDefinitionConfiguration : IEntityTypeConfiguration<ClassAbilityDefinition>
{
    public void Configure(EntityTypeBuilder<ClassAbilityDefinition> builder)
    {
        builder.ToTable("ClassAbilityDefinitions");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).IsRequired();
        builder.Property(a => a.MinLevel).IsRequired();
        builder.Property(a => a.ClassOwner).HasMaxLength(50).IsRequired();
        builder.HasIndex(a => a.ClassOwner);
        builder.OwnsMany(a => a.Effects, b => b.ToJson());
    }
}
