using WWN.Domain.Enums;

namespace WWN.Domain.Lookups;

public static class SkillNameCatalog
{
    private static readonly IReadOnlyDictionary<SkillName, LookupValue> Entries =
        new Dictionary<SkillName, LookupValue>
        {
            // Ordinals 0–15 are locked (stored as integers in focus/ability JSON blobs).
            // Ordinals 16–21 are append-safe.
            [SkillName.Connect] = new(
                Id: (int)SkillName.Connect,
                Code: nameof(SkillName.Connect),
                DisplayName: "Connect",
                Description: "Social networking, making contacts, and calling in favors.",
                SortOrder: 1),
            [SkillName.Know] = new(
                Id: (int)SkillName.Know,
                Code: nameof(SkillName.Know),
                DisplayName: "Know",
                Description: "Recalling general and obscure facts across many fields of knowledge.",
                SortOrder: 2),
            [SkillName.Lead] = new(
                Id: (int)SkillName.Lead,
                Code: nameof(SkillName.Lead),
                DisplayName: "Lead",
                Description: "Inspiring, directing, and coordinating others in pursuit of a goal.",
                SortOrder: 3),
            [SkillName.Magic] = new(
                Id: (int)SkillName.Magic,
                Code: nameof(SkillName.Magic),
                DisplayName: "Magic",
                Description: "Understanding and channeling magical arts; affects Effort pool size.",
                SortOrder: 4),
            [SkillName.Notice] = new(
                Id: (int)SkillName.Notice,
                Code: nameof(SkillName.Notice),
                DisplayName: "Notice",
                Description: "Perceiving the environment, detecting hidden things, and situational awareness.",
                SortOrder: 5),
            [SkillName.Perform] = new(
                Id: (int)SkillName.Perform,
                Code: nameof(SkillName.Perform),
                DisplayName: "Perform",
                Description: "Music, dance, oration, or other public performance.",
                SortOrder: 6),
            [SkillName.Pray] = new(
                Id: (int)SkillName.Pray,
                Code: nameof(SkillName.Pray),
                DisplayName: "Pray",
                Description: "Religious devotion, theological knowledge, and seeking divine favor.",
                SortOrder: 7),
            [SkillName.Punch] = new(
                Id: (int)SkillName.Punch,
                Code: nameof(SkillName.Punch),
                DisplayName: "Punch",
                Description: "Unarmed combat skill; used for attack rolls when fighting barehanded.",
                SortOrder: 8),
            [SkillName.Ride] = new(
                Id: (int)SkillName.Ride,
                Code: nameof(SkillName.Ride),
                DisplayName: "Ride",
                Description: "Handling and riding mounts under difficult or dangerous conditions.",
                SortOrder: 9),
            [SkillName.Sail] = new(
                Id: (int)SkillName.Sail,
                Code: nameof(SkillName.Sail),
                DisplayName: "Sail",
                Description: "Operating and navigating watercraft.",
                SortOrder: 10),
            [SkillName.Shoot] = new(
                Id: (int)SkillName.Shoot,
                Code: nameof(SkillName.Shoot),
                DisplayName: "Shoot",
                Description: "Ranged weapon combat skill; used for attack rolls with bows, crossbows, and firearms.",
                SortOrder: 11),
            [SkillName.Sneak] = new(
                Id: (int)SkillName.Sneak,
                Code: nameof(SkillName.Sneak),
                DisplayName: "Sneak",
                Description: "Stealth, infiltration, picking locks, and avoiding detection.",
                SortOrder: 12),
            [SkillName.Stab] = new(
                Id: (int)SkillName.Stab,
                Code: nameof(SkillName.Stab),
                DisplayName: "Stab",
                Description: "Melee weapon combat skill; used for attack rolls with swords, axes, and most hand weapons.",
                SortOrder: 13),
            [SkillName.Survive] = new(
                Id: (int)SkillName.Survive,
                Code: nameof(SkillName.Survive),
                DisplayName: "Survive",
                Description: "Wilderness survival, navigation, hunting, and living off the land.",
                SortOrder: 14),
            [SkillName.Trade] = new(
                Id: (int)SkillName.Trade,
                Code: nameof(SkillName.Trade),
                DisplayName: "Trade",
                Description: "Commerce, appraisal, negotiation, and understanding economic systems.",
                SortOrder: 15),
            [SkillName.Work] = new(
                Id: (int)SkillName.Work,
                Code: nameof(SkillName.Work),
                DisplayName: "Work",
                Description: "Practical crafts, physical labor, and tradesperson knowledge.",
                SortOrder: 16),
            [SkillName.Administer] = new(
                Id: (int)SkillName.Administer,
                Code: nameof(SkillName.Administer),
                DisplayName: "Administer",
                Description: "Managing organizations, bureaucracies, logistics, and complex operations.",
                SortOrder: 0),
            [SkillName.Convince] = new(
                Id: (int)SkillName.Convince,
                Code: nameof(SkillName.Convince),
                DisplayName: "Convince",
                Description: "Persuasion, manipulation, fast-talking, and changing minds.",
                SortOrder: 17),
            [SkillName.Craft] = new(
                Id: (int)SkillName.Craft,
                Code: nameof(SkillName.Craft),
                DisplayName: "Craft",
                Description: "Creating, repairing, and modifying physical goods and equipment.",
                SortOrder: 18),
            [SkillName.Exert] = new(
                Id: (int)SkillName.Exert,
                Code: nameof(SkillName.Exert),
                DisplayName: "Exert",
                Description: "Feats of raw physical strength, endurance, and athletic performance.",
                SortOrder: 19),
            [SkillName.Heal] = new(
                Id: (int)SkillName.Heal,
                Code: nameof(SkillName.Heal),
                DisplayName: "Heal",
                Description: "Medical care, treating wounds, curing illness, and stabilizing the dying.",
                SortOrder: 20),
            [SkillName.Custom] = new(
                Id: (int)SkillName.Custom,
                Code: nameof(SkillName.Custom),
                DisplayName: "Custom",
                Description: "A character-specific skill with a name defined by the player.",
                SortOrder: 99),
        };

    public static IReadOnlyList<LookupValue> All { get; } = Entries.Values
        .OrderBy(v => v.SortOrder)
        .ToArray();

    public static LookupValue Get(SkillName value) =>
        Entries.TryGetValue(value, out var v)
            ? v
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown SkillName.");
}
