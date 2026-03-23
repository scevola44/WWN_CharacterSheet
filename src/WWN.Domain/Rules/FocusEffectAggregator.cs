using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class FocusEffectAggregator
{
    public static int SumEffects(IEnumerable<Focus> foci, FocusEffectType type)
    {
        return foci.SelectMany(focus => focus.Effects)
                   .Where(effect => effect.Type == type)
                   .Sum(effect => effect.NumericValue);
    }

    public static int SumSkillEffects(IEnumerable<Focus> foci, SkillName skill)
    {
        return foci.SelectMany(focus => focus.Effects)
                   .Where(effect => effect.Type == FocusEffectType.SkillBonus 
                                    && effect.TargetSkill == skill)
                   .Sum(effect => effect.NumericValue);
    }

    public static int SumAttributeEffects(IEnumerable<Focus> foci, AttributeName attribute)
    {
        return foci.SelectMany(focus => focus.Effects)
                   .Where(effect => effect.Type == FocusEffectType.AttributeBonus 
                                    && effect.TargetAttribute == attribute)
                   .Sum(e => e.NumericValue);
    }
}
