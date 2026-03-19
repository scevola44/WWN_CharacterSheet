using WWN.Domain.Aggregates;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class SavingThrowCalculator
{
    public static int GetBaseSave(int level, bool isSpecialist)
    {
        return isSpecialist
            ? 16 - level
            : 15 - (level / 2);
    }

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
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    public static int GetSaveTarget(SaveType type, Character character, bool isSpecialist)
    {
        return GetBaseSave(character.Level, isSpecialist) - GetSaveModifier(type, character);
    }
}
