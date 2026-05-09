using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class ItemSlotTypeCatalog
{
    private static readonly IReadOnlyDictionary<ItemSlotType, LookupValue> Entries =
        new Dictionary<ItemSlotType, LookupValue>
        {
            [ItemSlotType.Stowed] = new(
                Id: (int)ItemSlotType.Stowed,
                Code: nameof(ItemSlotType.Stowed),
                DisplayName: "Stowed",
                Description: "Packed away; not immediately accessible. Counts toward stowed capacity (up to STR score items).",
                SortOrder: 0),
            [ItemSlotType.Readied] = new(
                Id: (int)ItemSlotType.Readied,
                Code: nameof(ItemSlotType.Readied),
                DisplayName: "Readied",
                Description: "At hand on belt, holster, or bandolier. Quick to draw or use. Counts toward readied capacity (up to STR/2 items).",
                SortOrder: 1),
            [ItemSlotType.Equipped] = new(
                Id: (int)ItemSlotType.Equipped,
                Code: nameof(ItemSlotType.Equipped),
                DisplayName: "Equipped",
                Description: "Worn or wielded — armor on the body, weapon in hand, shield strapped. Immediately usable; counts toward readied load.",
                SortOrder: 2),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(ItemSlotType value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown ItemSlotType.");
}
