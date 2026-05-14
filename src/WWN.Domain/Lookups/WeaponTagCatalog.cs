using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

// Each entry represents a single flag bit. Bitmask combinations are not catalogued;
// resolve them client-side by testing individual bits against this catalog.
public static class WeaponTagCatalog
{
    private static readonly IReadOnlyDictionary<WeaponTag, LookupValue> Entries =
        new Dictionary<WeaponTag, LookupValue>
        {
            [WeaponTag.TwoHanded] = new(
                Id: (int)WeaponTag.TwoHanded,
                Code: nameof(WeaponTag.TwoHanded),
                DisplayName: "Two-Handed",
                Description: "Requires both hands to wield; cancels the shield AC bonus while equipped.",
                SortOrder: 0),
            [WeaponTag.Subtle] = new(
                Id: (int)WeaponTag.Subtle,
                Code: nameof(WeaponTag.Subtle),
                DisplayName: "Subtle",
                Description: "Easily concealed; well-suited for surprise attacks.",
                SortOrder: 1),
            [WeaponTag.Long] = new(
                Id: (int)WeaponTag.Long,
                Code: nameof(WeaponTag.Long),
                DisplayName: "Long",
                Description: "Extended reach; can strike targets at the edge of melee range.",
                SortOrder: 2),
            [WeaponTag.Thrown] = new(
                Id: (int)WeaponTag.Thrown,
                Code: nameof(WeaponTag.Thrown),
                DisplayName: "Thrown",
                Description: "Can be thrown as a ranged attack in addition to melee use.",
                SortOrder: 3),
            [WeaponTag.AP] = new(
                Id: (int)WeaponTag.AP,
                Code: nameof(WeaponTag.AP),
                DisplayName: "Armor-Piercing",
                Description: "Partially ignores the target's armor bonus to AC.",
                SortOrder: 4),
            [WeaponTag.Reload] = new(
                Id: (int)WeaponTag.Reload,
                Code: nameof(WeaponTag.Reload),
                DisplayName: "Reload",
                Description: "Requires a reload action between shots.",
                SortOrder: 5),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(WeaponTag value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown or composite WeaponTag ({value}). Only individual flag bits are supported.");
}
