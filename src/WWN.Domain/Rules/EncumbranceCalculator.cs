using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class EncumbranceCalculator
{
    /// <summary>
    /// Max readied slots = STR score / 2 (integer division).
    /// Equipped items count toward this limit.
    /// </summary>
    public static int GetMaxReadied(int strScore) => strScore / 2;

    /// <summary>
    /// Max stowed slots = STR score (separate pool from readied).
    /// </summary>
    public static int GetMaxStowed(int strScore) => strScore;

    /// <summary>
    /// Total encumbrance used by Readied and Equipped items (Encumbrance × Quantity each).
    /// </summary>
    public static int GetReadiedLoad(IEnumerable<Item> inventory) =>
        inventory
            .Where(i => i.SlotType == ItemSlotType.Readied || i.SlotType == ItemSlotType.Equipped)
            .Sum(i => i.Encumbrance * i.Quantity);

    /// <summary>
    /// Total encumbrance used by Stowed items (Encumbrance × Quantity each).
    /// </summary>
    public static int GetStowedLoad(IEnumerable<Item> inventory) =>
        inventory
            .Where(i => i.SlotType == ItemSlotType.Stowed)
            .Sum(i => i.Encumbrance * i.Quantity);
}
