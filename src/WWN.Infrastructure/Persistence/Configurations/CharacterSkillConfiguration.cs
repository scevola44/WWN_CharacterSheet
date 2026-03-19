using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkill>
{
    public void Configure(EntityTypeBuilder<CharacterSkill> builder)
    {
        builder.ToTable("CharacterSkills");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.CustomName).HasMaxLength(100);
        builder.OwnsOne(s => s.Rank, r =>
        {
            r.Property(x => x.Level).HasColumnName("Level").IsRequired();
        });
    }
}
