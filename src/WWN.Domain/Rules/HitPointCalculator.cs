using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class HitPointCalculator
{
    public static int GetHitDieModifier(CharacterClass charClass, PartialClass? partialA,
        PartialClass? partialB)
    {
        if (charClass == CharacterClass.Warrior) return 2;
        if (charClass == CharacterClass.Mage) return -1;
        if (charClass == CharacterClass.Expert) return 0;

        // Adventurer: check partials
        bool hasWarrior = partialA == PartialClass.PartialWarrior ||
                          partialB == PartialClass.PartialWarrior;
        bool hasMage = partialA == PartialClass.PartialMage ||
                       partialB == PartialClass.PartialMage;

        if (hasWarrior) return 2;
        if (hasMage) return -1;
        return 0;
    }
}
