using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class EffortCommitmentCatalog
{
    private static readonly IReadOnlyDictionary<EffortCommitment, LookupValue> Entries =
        new Dictionary<EffortCommitment, LookupValue>
        {
            [EffortCommitment.None] = new(
                Id: (int)EffortCommitment.None,
                Code: nameof(EffortCommitment.None),
                DisplayName: "No effort",
                Description: "The Art has no Effort cost.",
                SortOrder: 0),
            [EffortCommitment.Scene] = new(
                Id: (int)EffortCommitment.Scene,
                Code: nameof(EffortCommitment.Scene),
                DisplayName: "Scene",
                Description: "Effort is committed for the scene; recovered when the scene ends.",
                SortOrder: 1),
            [EffortCommitment.Day] = new(
                Id: (int)EffortCommitment.Day,
                Code: nameof(EffortCommitment.Day),
                DisplayName: "Day",
                Description: "Effort is committed for the day; recovered after a full day's rest.",
                SortOrder: 2),
            [EffortCommitment.Sustained] = new(
                Id: (int)EffortCommitment.Sustained,
                Code: nameof(EffortCommitment.Sustained),
                DisplayName: "Sustained",
                Description: "Effort is committed indefinitely while the user maintains the Art; released explicitly.",
                SortOrder: 3)
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(EffortCommitment value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown EffortCommitment.");
}
