namespace WWN.Domain.ValueObjects;

public sealed record SkillRank
{
    public int Level { get; }

    public SkillRank(int level)
    {
        if (level is < -1 or > 4)
            throw new ArgumentOutOfRangeException(nameof(level), level, "Skill level must be -1 to 4.");
        Level = level;
    }

    private SkillRank() { } // EF Core
}
