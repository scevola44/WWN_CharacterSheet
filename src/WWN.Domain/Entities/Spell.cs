namespace WWN.Domain.Entities;

public class Spell
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SpellLevel { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? School { get; private set; }
    public string? Duration { get; private set; }
    public string? Range { get; private set; }

    public Spell(string name, int spellLevel, string description, string? school = null, string? duration = null, string? range = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Spell name is required.", nameof(name));
        if (spellLevel < 1 || spellLevel > 6)
            throw new ArgumentOutOfRangeException(nameof(spellLevel), "Spell level must be 1-6.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Spell description is required.", nameof(description));

        Id = Guid.NewGuid();
        Name = name;
        SpellLevel = spellLevel;
        Description = description;
        School = school;
        Duration = duration;
        Range = range;
    }

    private Spell() { } // EF Core
}
