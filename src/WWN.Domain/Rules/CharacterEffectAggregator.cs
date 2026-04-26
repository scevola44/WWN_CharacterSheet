using WWN.Domain.Aggregates;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Rules;

internal static class CharacterEffectAggregator
{
    internal static bool MatchesCondition(ICharacterEffect effect, FocusEffectCondition context, bool conditionalActive)
    {
        return effect.Condition switch
        {
            FocusEffectCondition.Always => true,
            FocusEffectCondition.Conditional => conditionalActive,
            _ => effect.Condition == context
        };
    }

    internal static int GetValue(ICharacterEffect effect, Character character) => effect.ValueType switch
    {
        FocusEffectValueType.Level => character.Level,
        FocusEffectValueType.HalfLevel => character.Level / 2,
        FocusEffectValueType.HalfLevelRoundedUp => (character.Level + 1) / 2,
        FocusEffectValueType.SkillLevel =>
            (character.GetSkillOrDefault(effect.TargetSkill!.Value)?.Rank.Level ?? -1)
            + FocusEffectAggregator.SumSkillEffects(character.Foci, effect.TargetSkill!.Value, character)
            + ClassAbilityEffectAggregator.SumSkillEffects(character.ClassAbilities, effect.TargetSkill!.Value, character),
        _ => effect.NumericValue
    };
}
