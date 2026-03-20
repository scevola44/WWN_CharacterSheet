using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Aggregates;

public class Character
{
    public Guid Id { get; private set; }

    // Identity
    public string Name { get; private set; } = string.Empty;
    public string? Background { get; private set; }
    public string? Origin { get; private set; }

    // Class
    public CharacterClass Class { get; private set; }
    public PartialClass? PartialClassA { get; private set; }
    public PartialClass? PartialClassB { get; private set; }
    public int Level { get; private set; }

    // HP
    public int MaxHitPoints { get; private set; }
    public int CurrentHitPoints { get; private set; }

    // Experience
    public int ExperiencePoints { get; private set; }

    // Notes
    public string? Notes { get; set; }

    // Collections
    private readonly List<CharacterAttribute> _attributes = new();
    private readonly List<CharacterSkill> _skills = new();
    private readonly List<Focus> _foci = new();
    private readonly List<Item> _inventory = new();

    public IReadOnlyList<CharacterAttribute> Attributes => _attributes.AsReadOnly();
    public IReadOnlyList<CharacterSkill> Skills => _skills.AsReadOnly();
    public IReadOnlyList<Focus> Foci => _foci.AsReadOnly();
    public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

    private Character() { } // EF Core

    public static Character Create(string name, CharacterClass charClass,
        Dictionary<AttributeName, int> scores,
        string? background = null, string? origin = null,
        PartialClass? partialA = null, PartialClass? partialB = null,
        int maxHitPoints = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name is required.", nameof(name));

        if (charClass == CharacterClass.Adventurer && (partialA is null || partialB is null))
            throw new ArgumentException("Adventurer class requires two partial classes.");

        if (charClass != CharacterClass.Adventurer && (partialA is not null || partialB is not null))
            throw new ArgumentException("Only Adventurer class uses partial classes.");

        var character = new Character
        {
            Id = Guid.NewGuid(),
            Name = name,
            Class = charClass,
            PartialClassA = partialA,
            PartialClassB = partialB,
            Background = background,
            Origin = origin,
            Level = 1,
            MaxHitPoints = maxHitPoints < 1 ? 1 : maxHitPoints,
            CurrentHitPoints = maxHitPoints < 1 ? 1 : maxHitPoints,
            ExperiencePoints = 0
        };

        // Initialize 6 attributes
        foreach (AttributeName attr in Enum.GetValues<AttributeName>())
        {
            if (!scores.TryGetValue(attr, out var score))
                throw new ArgumentException($"Missing score for {attr}.", nameof(scores));

            var charAttr = new CharacterAttribute(attr, score);
            character._attributes.Add(charAttr);
        }

        // Initialize 16 default skills at -1
        foreach (SkillName skill in Enum.GetValues<SkillName>())
        {
            if (skill == SkillName.Custom) continue;
            var charSkill = new CharacterSkill(skill, -1);
            character._skills.Add(charSkill);
        }

        return character;
    }

    // Attribute mutations
    public void SetAttribute(AttributeName attr, int score)
    {
        var charAttr = GetAttribute(attr);
        charAttr.SetScore(score);
    }

    public CharacterAttribute GetAttribute(AttributeName name)
    {
        return _attributes.FirstOrDefault(a => a.Name == name)
            ?? throw new InvalidOperationException($"Attribute {name} not found.");
    }

    // Skill mutations
    public void SetSkillRank(SkillName skill, int rank)
    {
        var charSkill = GetSkill(skill);
        charSkill.SetRank(rank);
    }

    public CharacterSkill GetSkill(SkillName name)
    {
        return _skills.FirstOrDefault(s => s.Name == name)
            ?? throw new InvalidOperationException($"Skill {name} not found.");
    }

    public CharacterSkill? GetSkillOrDefault(SkillName name)
    {
        return _skills.FirstOrDefault(s => s.Name == name);
    }

    public void AddCustomSkill(string name, int rank)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Custom skill name is required.", nameof(name));
        _skills.Add(new CharacterSkill(SkillName.Custom, rank, name));
    }

    // Focus mutations
    public void AddFocus(Focus focus)
    {
        _foci.Add(focus);
    }

    public void RemoveFocus(Guid focusId)
    {
        var focus = _foci.FirstOrDefault(f => f.Id == focusId)
            ?? throw new InvalidOperationException("Focus not found.");
        _foci.Remove(focus);
    }

    // HP mutations
    public void SetHitPoints(int max, int current)
    {
        if (max < 1) throw new ArgumentOutOfRangeException(nameof(max), "Max HP must be at least 1.");
        if (current < 0) throw new ArgumentOutOfRangeException(nameof(current), "Current HP cannot be negative.");
        if (current > max) throw new ArgumentException("Current HP cannot exceed max HP.");
        MaxHitPoints = max;
        CurrentHitPoints = current;
    }

    public void TakeDamage(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        CurrentHitPoints = Math.Max(0, CurrentHitPoints - amount);
    }

    public void Heal(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        CurrentHitPoints = Math.Min(MaxHitPoints, CurrentHitPoints + amount);
    }

    // Inventory mutations
    public void AddItem(Item item)
    {
        _inventory.Add(item);
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _inventory.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item not found.");
        _inventory.Remove(item);
    }

    public void EquipItem(Guid itemId)
    {
        var item = _inventory.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item not found.");
        item.SlotType = ItemSlotType.Equipped;
    }

    public void UnequipItem(Guid itemId)
    {
        var item = _inventory.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item not found.");
        item.SlotType = ItemSlotType.Stowed;
    }

    public void ReadyItem(Guid itemId)
    {
        var item = _inventory.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item not found.");
        item.SlotType = ItemSlotType.Readied;
    }

    // Query helpers
    public Weapon? GetEquippedWeapon()
    {
        return _inventory.OfType<Weapon>().FirstOrDefault(w => w.SlotType == ItemSlotType.Equipped);
    }

    public Armor? GetWornArmor()
    {
        return _inventory.OfType<Armor>().FirstOrDefault(a => a.SlotType == ItemSlotType.Equipped && !a.IsShield);
    }

    public Armor? GetEquippedShield()
    {
        return _inventory.OfType<Armor>().FirstOrDefault(a => a.SlotType == ItemSlotType.Equipped && a.IsShield);
    }

    // Level & XP
    public void SetLevel(int level)
    {
        if (level < 1 || level > 10)
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be 1-10.");
        Level = level;
    }

    public void SetExperiencePoints(int xp)
    {
        if (xp < 0) throw new ArgumentOutOfRangeException(nameof(xp));
        ExperiencePoints = xp;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name is required.", nameof(name));
        Name = name;
    }

    public void SetBackground(string? background) => Background = background;
    public void SetOrigin(string? origin) => Origin = origin;
}
