namespace WWN.Domain.Enums;

public enum FocusEffectValueType
{
    Static,              // use NumericValue as-is
    Level,               // use character.Level
    HalfLevel,           // use character.Level / 2 (rounds down)
    HalfLevelRoundedUp,  // use (character.Level + 1) / 2 (rounds up)
    SkillLevel           // use the rank of TargetSkill
}
