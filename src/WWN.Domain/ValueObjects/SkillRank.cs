namespace WWN.Domain.ValueObjects;

// TODO in Worlds Without Number, skills can also have partial points. Represent this logic with small squares next to the number, which represent the new property.
public sealed record SkillRank
{
    public int Level { get; }

    public SkillRank(int level)
    {
        if (level < -1 || level > 4)
            throw new ArgumentOutOfRangeException(nameof(level), level, "Skill level must be -1 to 4.");
        Level = level;
    }

    private SkillRank() { } // EF Core
}
