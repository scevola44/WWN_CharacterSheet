using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class ClassAbilityEffectAggregator
{
    public static int SumEffects(
        IEnumerable<ClassAbilityDefinition> abilities,
        FocusEffectType type,
        Character character,
        FocusEffectCondition context = FocusEffectCondition.Always)
    {
        return abilities
            .SelectMany(a => a.Effects)
            .Where(e => e.Type == type && CharacterEffectAggregator.MatchesCondition(e, context, false))
            .Sum(e => CharacterEffectAggregator.GetValue(e, character));
    }

    public static int SumSkillEffects(
        IEnumerable<ClassAbilityDefinition> abilities,
        SkillName skill,
        Character character)
    {
        return abilities
            .SelectMany(a => a.Effects)
            .Where(e => e.Type == FocusEffectType.SkillBonus
                        && e.TargetSkill == skill
                        && CharacterEffectAggregator.MatchesCondition(e, FocusEffectCondition.Always, false))
            .Sum(e => CharacterEffectAggregator.GetValue(e, character));
    }

    public static int SumAttributeEffects(
        IEnumerable<ClassAbilityDefinition> abilities,
        AttributeName attribute,
        Character character)
    {
        return abilities
            .SelectMany(a => a.Effects)
            .Where(e => e.Type == FocusEffectType.AttributeBonus
                        && e.TargetAttribute == attribute
                        && CharacterEffectAggregator.MatchesCondition(e, FocusEffectCondition.Always, false))
            .Sum(e => CharacterEffectAggregator.GetValue(e, character));
    }
}
