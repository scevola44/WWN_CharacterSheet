using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class FocusEffectAggregator
{
    public static int SumEffects(IEnumerable<Focus> foci, FocusEffectType type)
    {
        return foci.SelectMany(f => f.Effects)
                   .Where(e => e.Type == type)
                   .Sum(e => e.NumericValue);
    }

    public static int SumSkillEffects(IEnumerable<Focus> foci, SkillName skill)
    {
        return foci.SelectMany(f => f.Effects)
                   .Where(e => e.Type == FocusEffectType.SkillBonus && e.TargetSkill == skill)
                   .Sum(e => e.NumericValue);
    }

    public static int SumAttributeEffects(IEnumerable<Focus> foci, AttributeName attr)
    {
        return foci.SelectMany(f => f.Effects)
                   .Where(e => e.Type == FocusEffectType.AttributeBonus && e.TargetAttribute == attr)
                   .Sum(e => e.NumericValue);
    }
}
