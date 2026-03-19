using WWN.Domain.Enums;

namespace WWN.Domain.Entities;

public class Armor : Item
{
    public int AcBonus { get; private set; }
    public bool IsShield { get; private set; }

    public Armor(string name, int encumbrance, int acBonus, bool isShield = false,
        ItemSlotType slotType = ItemSlotType.Stowed, string? description = null)
        : base(name, encumbrance, slotType, 1, description)
    {
        AcBonus = acBonus;
        IsShield = isShield;
    }

    private Armor() { } // EF Core
}
