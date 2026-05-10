using WWN.Domain.Enums;

namespace WWN.Domain.Entities;

public class Item
{
    public Guid Id { get; protected set; }
    public Guid CharacterId { get; protected set; }
    public string Name { get; protected set; } = string.Empty;
    public string? Description { get; protected set; }
    public int Encumbrance { get; protected set; }
    public ItemSlotType SlotType { get; internal set; }
    public int Quantity { get; internal set; }

    public Item(string name, int encumbrance, ItemSlotType slotType = ItemSlotType.Stowed,
        int quantity = 1, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Encumbrance = encumbrance;
        SlotType = slotType;
        Quantity = quantity;
        Description = description;
    }

    public virtual void Update(string name, int encumbrance, int quantity, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Item name is required.", nameof(name));
        if (encumbrance < 0)
            throw new ArgumentOutOfRangeException(nameof(encumbrance), "Encumbrance cannot be negative.");
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        Name = name;
        Encumbrance = encumbrance;
        Quantity = quantity;
        Description = description;
    }

    protected Item() { } // EF Core
}
