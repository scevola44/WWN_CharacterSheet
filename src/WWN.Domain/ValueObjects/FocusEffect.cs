using WWN.Domain.Enums;

namespace WWN.Domain.ValueObjects;

public sealed record FocusEffect(
    FocusEffectType Type,
    int NumericValue,
    SkillName? TargetSkill = null,
    AttributeName? TargetAttribute = null,
    string? Description = null);
