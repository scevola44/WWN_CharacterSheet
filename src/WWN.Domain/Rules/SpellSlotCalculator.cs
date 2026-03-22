using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

/// <summary>
/// Calculates available spell slots per spell level for Mage and Partial Mage classes.
/// Slot counts are indexed by spell level (0-5) representing levels 1-6.
/// </summary>
public static class SpellSlotCalculator
{
    // Full Mage spell slot progression by character level
    private static readonly int[][] MageSlotTable =
    {
        // Level 1
        [1, 0, 0, 0, 0, 0],
        // Level 2
        [2, 0, 0, 0, 0, 0],
        // Level 3
        [2, 1, 0, 0, 0, 0],
        // Level 4
        [2, 2, 0, 0, 0, 0],
        // Level 5
        [2, 2, 1, 0, 0, 0],
        // Level 6
        [2, 2, 2, 0, 0, 0],
        // Level 7
        [2, 2, 2, 1, 0, 0],
        // Level 8
        [2, 2, 2, 2, 0, 0],
        // Level 9
        [2, 2, 2, 2, 1, 0],
        // Level 10
        [2, 2, 2, 2, 2, 1],
    };

    // Partial Mage (Adventurer) spell slot progression by character level
    private static readonly int[][] PartialMageSlotTable =
    {
        // Level 1
        [0, 0, 0, 0, 0, 0],
        // Level 2
        [1, 0, 0, 0, 0, 0],
        // Level 3
        [1, 0, 0, 0, 0, 0],
        // Level 4
        [1, 1, 0, 0, 0, 0],
        // Level 5
        [1, 1, 0, 0, 0, 0],
        // Level 6
        [1, 1, 1, 0, 0, 0],
        // Level 7
        [1, 1, 1, 0, 0, 0],
        // Level 8
        [1, 1, 1, 1, 0, 0],
        // Level 9
        [1, 1, 1, 1, 0, 0],
        // Level 10
        [1, 1, 1, 1, 1, 0],
    };

    /// <summary>
    /// Calculate spell slots available for a character.
    /// </summary>
    /// <param name="charClass">Character class (Mage or Adventurer)</param>
    /// <param name="level">Character level (1-10)</param>
    /// <param name="intModifier">Intelligence attribute modifier (for bonus 1st-level slots)</param>
    /// <returns>Array of 6 integers representing available slots for spell levels 1-6</returns>
    public static int[] CalculateSlots(CharacterClass charClass, int level, int intModifier)
    {
        if (level < 1 || level > 10)
            throw new ArgumentOutOfRangeException(nameof(level), "Character level must be 1-10.");

        var table = charClass == CharacterClass.Mage ? MageSlotTable : PartialMageSlotTable;
        var slots = (int[])table[level - 1].Clone();

        // Add INT modifier to 1st-level spell slots (for Mage only)
        if (charClass == CharacterClass.Mage && intModifier > 0)
        {
            slots[0] += intModifier;
        }

        return slots;
    }

    /// <summary>
    /// Calculate spell slots for an Adventurer with a Partial Mage class.
    /// </summary>
    public static int[] CalculateSlotsForPartialMage(int level, int intModifier)
    {
        if (level < 1 || level > 10)
            throw new ArgumentOutOfRangeException(nameof(level), "Character level must be 1-10.");

        var slots = (int[])PartialMageSlotTable[level - 1].Clone();

        // Partial Mages don't get INT modifier bonus to 1st-level slots
        return slots;
    }
}
