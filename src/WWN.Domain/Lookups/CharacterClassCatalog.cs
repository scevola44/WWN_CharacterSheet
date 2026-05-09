using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class CharacterClassCatalog
{
    private static readonly IReadOnlyDictionary<CharacterClass, LookupValue> Entries =
        new Dictionary<CharacterClass, LookupValue>
        {
            [CharacterClass.Warrior] = new(
                Id: (int)CharacterClass.Warrior,
                Code: nameof(CharacterClass.Warrior),
                DisplayName: "Warrior",
                Description: "Combat specialist. Full BAB (= level). +2 HP per level. Gains bonus combat foci and excels in all forms of fighting.",
                SortOrder: 0),
            [CharacterClass.Expert] = new(
                Id: (int)CharacterClass.Expert,
                Code: nameof(CharacterClass.Expert),
                DisplayName: "Expert",
                Description: "Skill specialist. Half BAB (level/2). Gains bonus non-combat foci and extra skill picks at each level.",
                SortOrder: 1),
            [CharacterClass.Mage] = new(
                Id: (int)CharacterClass.Mage,
                Code: nameof(CharacterClass.Mage),
                DisplayName: "Mage",
                Description: "Spellcaster. Half BAB (level/2). −1 HP per level. Full spell slot progression and Effort pool for Arts.",
                SortOrder: 2),
            [CharacterClass.Adventurer] = new(
                Id: (int)CharacterClass.Adventurer,
                Code: nameof(CharacterClass.Adventurer),
                DisplayName: "Adventurer",
                Description: "Hybrid. Combines two distinct partial classes, gaining limited versions of both parent class abilities.",
                SortOrder: 3),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(CharacterClass value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown CharacterClass.");
}
