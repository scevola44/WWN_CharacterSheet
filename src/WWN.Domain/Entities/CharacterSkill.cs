using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

public class CharacterSkill
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public SkillName Name { get; private set; }
    public string? CustomName { get; private set; }
    public SkillRank Rank { get; private set; } = null!;

    public CharacterSkill(SkillName name, int rank, string? customName = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        CustomName = name == SkillName.Custom ? customName : null;
        Rank = new SkillRank(rank);
    }

    private CharacterSkill() { } // EF Core

    public void SetRank(int level) => Rank = new SkillRank(level);
}
