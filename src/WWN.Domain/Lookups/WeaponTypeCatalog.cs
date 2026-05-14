using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class WeaponTypeCatalog
{
    private static readonly IReadOnlyDictionary<WeaponType, LookupValue> Entries =
        new Dictionary<WeaponType, LookupValue>
        {
            [WeaponType.Melee] = new(
                Id: (int)WeaponType.Melee,
                Code: nameof(WeaponType.Melee),
                DisplayName: "Melee",
                Description: "Usable in close combat; defaults to the Stab combat skill.",
                SortOrder: 0),
            [WeaponType.Ranged] = new(
                Id: (int)WeaponType.Ranged,
                Code: nameof(WeaponType.Ranged),
                DisplayName: "Ranged",
                Description: "Usable at range; defaults to the Shoot combat skill.",
                SortOrder: 1),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(WeaponType value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown WeaponType ({value}).");
}
