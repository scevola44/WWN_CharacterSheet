namespace WWN.Domain.Enums;

public enum FocusEffectValueType
{
    Static,     // use NumericValue as-is
    Level,      // use character.Level
    HalfLevel,  // use character.Level / 2
    SkillLevel  // use the rank of TargetSkill
}
