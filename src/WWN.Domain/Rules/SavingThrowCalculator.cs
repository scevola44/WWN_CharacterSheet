using WWN.Domain.Aggregates;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class SavingThrowCalculator
{
    // All WWN saves use 16 - level - attribute_mod (Physical, Evasion, Mental).
    // There is no generalist formula; the old isSpecialist parameter has been removed.
    public static int GetBaseSave(int level) => 16 - level;

    public static int GetSaveModifier(SaveType type, Character character)
    {
        return type switch
        {
            SaveType.Physical => Math.Max(
                character.GetAttribute(AttributeName.Strength).Modifier,
                character.GetAttribute(AttributeName.Constitution).Modifier),
            SaveType.Evasion => Math.Max(
                character.GetAttribute(AttributeName.Dexterity).Modifier,
                character.GetAttribute(AttributeName.Intelligence).Modifier),
            SaveType.Mental => Math.Max(
                character.GetAttribute(AttributeName.Wisdom).Modifier,
                character.GetAttribute(AttributeName.Charisma).Modifier),
            SaveType.Luck => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public static int GetSaveTarget(SaveType type, Character character)
    {
        return GetBaseSave(character.Level) - GetSaveModifier(type, character);
    }
}
