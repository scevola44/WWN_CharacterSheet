using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Rules;

public static class FocusEffectAggregator
{
    public static int SumEffects(
        IEnumerable<Focus> foci,
        FocusEffectType type,
        Character character,
        FocusEffectCondition context = FocusEffectCondition.Always)
    {
        return foci.SelectMany(focus => focus.Effects.Select(e => (focus, e)))
                   .Where(x => x.e.Type == type && MatchesCondition(x.e, context, x.focus.ConditionalActive))
                   .Sum(x => GetValue(x.e, character));
    }

    public static int SumSkillEffects(IEnumerable<Focus> foci, SkillName skill, Character character)
    {
        return foci.SelectMany(focus => focus.Effects.Select(e => (focus, e)))
                   .Where(x => x.e.Type == FocusEffectType.SkillBonus
                                && x.e.TargetSkill == skill
                                && MatchesCondition(x.e, FocusEffectCondition.Always, x.focus.ConditionalActive))
                   .Sum(x => GetValue(x.e, character));
    }

    public static int SumAttributeEffects(IEnumerable<Focus> foci, AttributeName attribute, Character character)
    {
        return foci.SelectMany(focus => focus.Effects.Select(e => (focus, e)))
                   .Where(x => x.e.Type == FocusEffectType.AttributeBonus
                                && x.e.TargetAttribute == attribute
                                && MatchesCondition(x.e, FocusEffectCondition.Always, x.focus.ConditionalActive))
                   .Sum(x => GetValue(x.e, character));
    }

    private static bool MatchesCondition(FocusEffect effect, FocusEffectCondition context, bool conditionalActive)
    {
        return effect.Condition switch
        {
            FocusEffectCondition.Always => true,
            FocusEffectCondition.Conditional => conditionalActive,
            _ => effect.Condition == context
        };
    }

    private static int GetValue(FocusEffect effect, Character character) => effect.ValueType switch
    {
        FocusEffectValueType.Level => character.Level,
        FocusEffectValueType.HalfLevel => character.Level / 2,
        FocusEffectValueType.SkillLevel => character.GetSkillOrDefault(effect.TargetSkill!.Value)?.Rank.Level ?? -1,
        _ => effect.NumericValue
    };
}
