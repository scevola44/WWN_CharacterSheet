using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class AttributeNameCatalog
{
    private static readonly IReadOnlyDictionary<AttributeName, LookupValue> Entries =
        new Dictionary<AttributeName, LookupValue>
        {
            // Ordinals 0–5 are locked (stored as integers in focus/ability JSON blobs).
            [AttributeName.Strength] = new(
                Id: (int)AttributeName.Strength,
                Code: nameof(AttributeName.Strength),
                DisplayName: "STR",
                Description: "Strength — physical power, melee attacks, and encumbrance capacity. Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 0),
            [AttributeName.Dexterity] = new(
                Id: (int)AttributeName.Dexterity,
                Code: nameof(AttributeName.Dexterity),
                DisplayName: "DEX",
                Description: "Dexterity — agility, reflexes, ranged attacks, and AC. Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 1),
            [AttributeName.Constitution] = new(
                Id: (int)AttributeName.Constitution,
                Code: nameof(AttributeName.Constitution),
                DisplayName: "CON",
                Description: "Constitution — endurance, health, and System Strain capacity (max strain = CON score). Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 2),
            [AttributeName.Intelligence] = new(
                Id: (int)AttributeName.Intelligence,
                Code: nameof(AttributeName.Intelligence),
                DisplayName: "INT",
                Description: "Intelligence — memory, reasoning, magical aptitude, and Effort pool size. Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 3),
            [AttributeName.Wisdom] = new(
                Id: (int)AttributeName.Wisdom,
                Code: nameof(AttributeName.Wisdom),
                DisplayName: "WIS",
                Description: "Wisdom — perception, willpower, and common sense. Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 4),
            [AttributeName.Charisma] = new(
                Id: (int)AttributeName.Charisma,
                Code: nameof(AttributeName.Charisma),
                DisplayName: "CHA",
                Description: "Charisma — presence, force of personality, and social grace. Modifier: 3=−2, 4–7=−1, 8–13=0, 14–17=+1, 18=+2.",
                SortOrder: 5),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(AttributeName value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown AttributeName.");
}
