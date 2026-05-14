using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

// Each entry represents a single flag bit. Bitmask combinations are not catalogued;
// resolve them client-side by testing individual bits against this catalog.
public static class WeaponTagCatalog
{
    private static readonly IReadOnlyDictionary<WeaponTag, LookupValue> Entries =
        new Dictionary<WeaponTag, LookupValue>
        {
            [WeaponTag.Melee] = new(
                Id: (int)WeaponTag.Melee,
                Code: nameof(WeaponTag.Melee),
                DisplayName: "Melee",
                Description: "Usable in close combat.",
                SortOrder: 0),
            [WeaponTag.Ranged] = new(
                Id: (int)WeaponTag.Ranged,
                Code: nameof(WeaponTag.Ranged),
                DisplayName: "Ranged",
                Description: "Usable at range; defaults to the Shoot combat skill.",
                SortOrder: 1),
            [WeaponTag.TwoHanded] = new(
                Id: (int)WeaponTag.TwoHanded,
                Code: nameof(WeaponTag.TwoHanded),
                DisplayName: "Two-Handed",
                Description: "Requires two hands to use in combat. Ranged two-handed weapons cannot be fired effectively while an enemy is within melee range.",
                SortOrder: 2,
                Abbreviation: "2H"),
            [WeaponTag.AP] = new(
                Id: (int)WeaponTag.AP,
                Code: nameof(WeaponTag.AP),
                DisplayName: "Armor-Piercing",
                Description: "Ignores non-magical hides, armor, and shields for purposes of hit rolls.",
                SortOrder: 3,
                Abbreviation: "AP"),
            [WeaponTag.Fixed] = new(
                Id: (int)WeaponTag.Fixed,
                Code: nameof(WeaponTag.Fixed),
                DisplayName: "Fixed",
                Description: "Too heavy and clumsy to use without a fixed position and at least five minutes to entrench.",
                SortOrder: 4,
                Abbreviation: "FX"),
            [WeaponTag.Long] = new(
                Id: (int)WeaponTag.Long,
                Code: nameof(WeaponTag.Long),
                DisplayName: "Long",
                Description: "Allows melee attacks at targets up to 10 feet distant, even if an ally is in the way. The wielder still needs to be within 5 feet of a foe to count as being in melee.",
                SortOrder: 5,
                Abbreviation: "L"),
            [WeaponTag.LessLethal] = new(
                Id: (int)WeaponTag.LessLethal,
                Code: nameof(WeaponTag.LessLethal),
                DisplayName: "Less Lethal",
                Description: "Foes brought to zero hit points by this weapon can always be left alive at the wielder's discretion.",
                SortOrder: 6,
                Abbreviation: "LL"),
            [WeaponTag.Numerous] = new(
                Id: (int)WeaponTag.Numerous,
                Code: nameof(WeaponTag.Numerous),
                DisplayName: "Numerous",
                Description: "Five of these count as only one Readied item.",
                SortOrder: 7,
                Abbreviation: "N"),
            [WeaponTag.PreciselyMurderous] = new(
                Id: (int)WeaponTag.PreciselyMurderous,
                Code: nameof(WeaponTag.PreciselyMurderous),
                DisplayName: "Precisely Murderous",
                Description: "When used for an Execution Attack, applies an additional −1 penalty to the Physical save and does double damage even if the save succeeds.",
                SortOrder: 8,
                Abbreviation: "PM"),
            [WeaponTag.Reload] = new(
                Id: (int)WeaponTag.Reload,
                Code: nameof(WeaponTag.Reload),
                DisplayName: "Reload",
                Description: "Takes a Move action to reload. With Shoot-1 or better, can be reloaded as an On Turn action instead.",
                SortOrder: 9,
                Abbreviation: "R"),
            [WeaponTag.Subtle] = new(
                Id: (int)WeaponTag.Subtle,
                Code: nameof(WeaponTag.Subtle),
                DisplayName: "Subtle",
                Description: "Can be easily hidden in ordinary clothing or concealed in jewelry.",
                SortOrder: 10,
                Abbreviation: "S"),
            [WeaponTag.SlowReload] = new(
                Id: (int)WeaponTag.SlowReload,
                Code: nameof(WeaponTag.SlowReload),
                DisplayName: "Slow Reload",
                Description: "Takes a Main Action to reload.",
                SortOrder: 11,
                Abbreviation: "SR"),
            [WeaponTag.SingleShot] = new(
                Id: (int)WeaponTag.SingleShot,
                Code: nameof(WeaponTag.SingleShot),
                DisplayName: "Single Shot",
                Description: "Takes ten rounds to reload; reloading is spoiled if an enemy melees the wielder.",
                SortOrder: 12,
                Abbreviation: "SS"),
            [WeaponTag.Throwable] = new(
                Id: (int)WeaponTag.Throwable,
                Code: nameof(WeaponTag.Throwable),
                DisplayName: "Throwable",
                Description: "Can be used in melee or thrown to the listed range, but does no Shock when thrown. Throwing while a foe is in melee range applies a −4 penalty to the attack roll.",
                SortOrder: 13,
                Abbreviation: "T"),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(WeaponTag value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown or composite WeaponTag ({value}). Only individual flag bits are supported.");
}
