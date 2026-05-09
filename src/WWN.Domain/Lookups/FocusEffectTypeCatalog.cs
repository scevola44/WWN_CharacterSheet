using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class FocusEffectTypeCatalog
{
    private static readonly IReadOnlyDictionary<FocusEffectType, LookupValue> Entries =
        new Dictionary<FocusEffectType, LookupValue>
        {
            // Ordinals 0–9 are locked (stored as integers in focus/ability JSON blobs).
            [FocusEffectType.SkillBonus] = new(
                Id: (int)FocusEffectType.SkillBonus,
                Code: nameof(FocusEffectType.SkillBonus),
                DisplayName: "Skill Bonus",
                Description: "Adds a bonus to checks with a specific skill.",
                SortOrder: 0),
            [FocusEffectType.AttributeBonus] = new(
                Id: (int)FocusEffectType.AttributeBonus,
                Code: nameof(FocusEffectType.AttributeBonus),
                DisplayName: "Attribute Bonus",
                Description: "Adds a bonus to a specific attribute score.",
                SortOrder: 1),
            [FocusEffectType.AttackBonus] = new(
                Id: (int)FocusEffectType.AttackBonus,
                Code: nameof(FocusEffectType.AttackBonus),
                DisplayName: "Attack Bonus",
                Description: "Adds a bonus to attack rolls, optionally restricted by weapon condition.",
                SortOrder: 2),
            [FocusEffectType.DamageBonus] = new(
                Id: (int)FocusEffectType.DamageBonus,
                Code: nameof(FocusEffectType.DamageBonus),
                DisplayName: "Damage Bonus",
                Description: "Adds a bonus to damage rolls, optionally restricted by weapon condition.",
                SortOrder: 3),
            [FocusEffectType.AcBonus] = new(
                Id: (int)FocusEffectType.AcBonus,
                Code: nameof(FocusEffectType.AcBonus),
                DisplayName: "AC Bonus",
                Description: "Adds a bonus to Armor Class.",
                SortOrder: 4),
            [FocusEffectType.ShockBonus] = new(
                Id: (int)FocusEffectType.ShockBonus,
                Code: nameof(FocusEffectType.ShockBonus),
                DisplayName: "Shock Bonus",
                Description: "Adds a bonus to Shock damage dealt on a miss against low-AC targets.",
                SortOrder: 5),
            [FocusEffectType.HpBonus] = new(
                Id: (int)FocusEffectType.HpBonus,
                Code: nameof(FocusEffectType.HpBonus),
                DisplayName: "HP Bonus",
                Description: "Adds a bonus to maximum Hit Points.",
                SortOrder: 6),
            [FocusEffectType.SaveBonus] = new(
                Id: (int)FocusEffectType.SaveBonus,
                Code: nameof(FocusEffectType.SaveBonus),
                DisplayName: "Save Bonus",
                Description: "Adds a bonus to saving throw rolls.",
                SortOrder: 7),
            [FocusEffectType.Initiative] = new(
                Id: (int)FocusEffectType.Initiative,
                Code: nameof(FocusEffectType.Initiative),
                DisplayName: "Initiative",
                Description: "Modifies the initiative roll order.",
                SortOrder: 8),
            [FocusEffectType.Custom] = new(
                Id: (int)FocusEffectType.Custom,
                Code: nameof(FocusEffectType.Custom),
                DisplayName: "Custom",
                Description: "A free-text effect not captured by the standard categories; described in the effect's Description field.",
                SortOrder: 9),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(FocusEffectType value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown FocusEffectType.");
}
