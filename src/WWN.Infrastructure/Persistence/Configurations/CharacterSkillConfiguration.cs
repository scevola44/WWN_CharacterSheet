using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence.Configurations;

public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkill>
{
    public void Configure(EntityTypeBuilder<CharacterSkill> builder)
    {
        builder.ToTable("CharacterSkills");
        builder.HasKey(skill => skill.Id);
        builder.Property(skill => skill.Name).HasConversion<string>().HasMaxLength(20);
        builder.Property(skill => skill.CustomName).HasMaxLength(100);
        builder.OwnsOne(skill => skill.Rank, r =>
        {
            r.Property(skillRank => skillRank.Level).HasColumnName("Level").IsRequired();
        });
    }
}
