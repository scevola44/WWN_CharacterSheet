using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class SaveTypeCatalog
{
    private static readonly IReadOnlyDictionary<SaveType, LookupValue> Entries =
        new Dictionary<SaveType, LookupValue>
        {
            [SaveType.Physical] = new(
                Id: (int)SaveType.Physical,
                Code: nameof(SaveType.Physical),
                DisplayName: "Physical",
                Description: "Resist physical trauma, poison, and bodily harm. Uses max(STR mod, CON mod). Target = 16 − level − modifier.",
                SortOrder: 0),
            [SaveType.Evasion] = new(
                Id: (int)SaveType.Evasion,
                Code: nameof(SaveType.Evasion),
                DisplayName: "Evasion",
                Description: "Dodge area effects, traps, and hazards. Uses max(DEX mod, INT mod). Target = 16 − level − modifier.",
                SortOrder: 1),
            [SaveType.Mental] = new(
                Id: (int)SaveType.Mental,
                Code: nameof(SaveType.Mental),
                DisplayName: "Mental",
                Description: "Resist mental assault, compulsion, and fear. Uses max(WIS mod, CHA mod). Target = 16 − level − modifier.",
                SortOrder: 2),
            [SaveType.Luck] = new(
                Id: (int)SaveType.Luck,
                Code: nameof(SaveType.Luck),
                DisplayName: "Luck",
                Description: "Pure fortune. No attribute modifier. Target = 16 − level.",
                SortOrder: 3),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(SaveType value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown SaveType.");
}
