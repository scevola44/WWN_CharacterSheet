using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

public class Weapon : Item
{
    public DamageDie DamageDie { get; private set; } = null!;
    public AttributeName AttributeModifier { get; private set; }
    public ShockInfo? Shock { get; private set; }
    public WeaponTag Tags { get; private set; }

    public Weapon(string name, int encumbrance, DamageDie damageDie,
        AttributeName attributeModifier, WeaponTag tags,
        ShockInfo? shock = null, ItemSlotType slotType = ItemSlotType.Stowed,
        string? description = null)
        : base(name, encumbrance, slotType, 1, description)
    {
        DamageDie = damageDie;
        AttributeModifier = attributeModifier;
        Tags = tags;
        Shock = shock;
    }

    private Weapon() { } // EF Core
}
