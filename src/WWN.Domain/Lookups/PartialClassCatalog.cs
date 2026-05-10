using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class PartialClassCatalog
{
    private static readonly IReadOnlyDictionary<PartialClass, LookupValue> Entries =
        new Dictionary<PartialClass, LookupValue>
        {
            [PartialClass.PartialWarrior] = new(
                Id: (int)PartialClass.PartialWarrior,
                Code: nameof(PartialClass.PartialWarrior),
                DisplayName: "Partial Warrior",
                Description: "Improved BAB (level/2 + 1). Access to bonus combat foci. The Adventurer's combat-oriented partial class.",
                SortOrder: 0),
            [PartialClass.PartialExpert] = new(
                Id: (int)PartialClass.PartialExpert,
                Code: nameof(PartialClass.PartialExpert),
                DisplayName: "Partial Expert",
                Description: "Bonus non-combat foci and extra skill picks at a reduced rate. The Adventurer's skill-oriented partial class.",
                SortOrder: 1),
            [PartialClass.PartialMage] = new(
                Id: (int)PartialClass.PartialMage,
                Code: nameof(PartialClass.PartialMage),
                DisplayName: "Partial Mage",
                Description: "Reduced spell slot progression (starts at level 1 with no slots). Effort pool for Arts based on INT and Magic rank.",
                SortOrder: 2),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(PartialClass value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown PartialClass.");
}
