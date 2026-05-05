using WWN.Domain.Aggregates;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

/// <summary>
/// Calculates the maximum Effort pool for a character.
/// Effort = 1 + INT modifier + Magic skill rank, available to Mages and to
/// Adventurers with at least one Partial Mage slot.
/// </summary>
public static class EffortPoolCalculator
{
    public static int CalculateMax(Character character)
    {
        if (!HasArts(character)) return 0;

        var intModifier = character.GetAttribute(AttributeName.Intelligence).Modifier;
        var magicSkill = character.GetSkillOrDefault(SkillName.Magic);
        // Untrained skills sit at -1 in this codebase; treat as 0 for the formula.
        var magicRank = Math.Max(0, magicSkill?.Rank.Level ?? 0);

        return Math.Max(1, 1 + intModifier + magicRank);
    }

    public static bool HasArts(Character character)
    {
        if (character.Class == CharacterClass.Mage) return true;
        return character.PartialClassA == PartialClass.PartialMage
            || character.PartialClassB == PartialClass.PartialMage;
    }
}
