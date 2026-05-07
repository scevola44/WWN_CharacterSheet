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

    public void Update(string name, int encumbrance, int acBonus, bool isShield = false, string? description = null)
    {
        base.Update(name, encumbrance, 1, description);
        if (acBonus < -2 || acBonus > 20)
            throw new ArgumentOutOfRangeException(nameof(acBonus), "AC bonus must be between -2 and 20.");
        AcBonus = acBonus;
        IsShield = isShield;
    }

    private Armor() { } // EF Core
}
