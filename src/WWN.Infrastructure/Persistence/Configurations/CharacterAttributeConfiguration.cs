using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterAttributeConfiguration : IEntityTypeConfiguration<CharacterAttribute>
{
    public void Configure(EntityTypeBuilder<CharacterAttribute> builder)
    {
        builder.ToTable("CharacterAttributes");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasConversion<string>().HasMaxLength(20);
        builder.OwnsOne(a => a.Score, s =>
        {
            s.Property(x => x.Value).HasColumnName("Score").IsRequired();
        });
    }
}
