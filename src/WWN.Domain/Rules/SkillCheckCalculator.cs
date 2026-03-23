using WWN.Domain.Aggregates;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

// TODO this is not used. It would be nice to have a dice roller with the option of selecting the relevant attribute.
public static class SkillCheckCalculator
{
    public static int GetSkillCheckModifier(
        Character character, 
        SkillName skill,
        AttributeName attribute)
    {
        int attrMod = character.GetAttribute(attribute).Modifier;
        var charSkill = character.GetSkillOrDefault(skill);
        int skillLevel = charSkill?.Rank.Level ?? -1;
        int focusSkillBonus = FocusEffectAggregator.SumSkillEffects(character.Foci, skill);
        return attrMod + skillLevel + focusSkillBonus;
    }
}
