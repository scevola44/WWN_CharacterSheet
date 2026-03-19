using WWN.Domain.Aggregates;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class SkillCheckCalculator
{
    public static int GetSkillCheckModifier(Character character, SkillName skill,
        AttributeName attribute)
    {
        int attrMod = character.GetAttribute(attribute).Modifier;
        var charSkill = character.GetSkillOrDefault(skill);
        int skillLevel = charSkill?.Rank.Level ?? -1;
        int focusSkillBonus = FocusEffectAggregator.SumSkillEffects(character.Foci, skill);
        return attrMod + skillLevel + focusSkillBonus;
    }
}
