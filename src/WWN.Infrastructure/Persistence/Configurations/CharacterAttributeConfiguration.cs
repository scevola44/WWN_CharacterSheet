using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterAttributeConfiguration : IEntityTypeConfiguration<CharacterAttribute>
{
    public void Configure(EntityTypeBuilder<CharacterAttribute> builder)
    {
        builder.ToTable("CharacterAttributes");
        builder.HasKey(attribute => attribute.Id);
        builder.Property(attribute => attribute.Name).HasConversion<string>().HasMaxLength(20);
        builder.OwnsOne(attribute => attribute.Score, s =>
        {
            s.Property(score => score.Value).HasColumnName("Score").IsRequired();
        });
    }
}
