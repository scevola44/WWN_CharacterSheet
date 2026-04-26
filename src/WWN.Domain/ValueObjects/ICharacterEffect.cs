using WWN.Domain.Enums;

namespace WWN.Domain.ValueObjects;

public interface ICharacterEffect
{
    FocusEffectType Type { get; }
    int NumericValue { get; }
    FocusEffectValueType ValueType { get; }
    FocusEffectCondition Condition { get; }
    SkillName? TargetSkill { get; }
    AttributeName? TargetAttribute { get; }
    string? Description { get; }
}
