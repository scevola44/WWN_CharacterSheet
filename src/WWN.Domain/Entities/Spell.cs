namespace WWN.Domain.Entities;

public class Spell
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SpellLevel { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? Summary { get; private set; }

    public Spell(string name, int spellLevel, string description, string? summary = null)
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
        Summary = summary;
    }

    public void Update(string name, int spellLevel, string? summary, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Spell name is required.", nameof(name));
        if (spellLevel < 1 || spellLevel > 6)
            throw new ArgumentOutOfRangeException(nameof(spellLevel), "Spell level must be 1-6.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Spell description is required.", nameof(description));

        Name = name;
        SpellLevel = spellLevel;
        Description = description;
        Summary = summary;
    }

    private Spell() { } // EF Core
}
