using WWN.Domain.Enums;

namespace WWN.Domain.ValueObjects;

public sealed record FocusEffect(
    FocusEffectType Type,
    int NumericValue,
    FocusEffectValueType ValueType = FocusEffectValueType.Static,
    FocusEffectCondition Condition = FocusEffectCondition.Always,
    SkillName? TargetSkill = null,
    AttributeName? TargetAttribute = null,
    string? Description = null);
