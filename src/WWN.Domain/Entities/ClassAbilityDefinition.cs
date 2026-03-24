using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

public class ClassAbilityDefinition
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int MinLevel { get; private set; }
    public string ClassOwner { get; private set; } = string.Empty;
    public List<ClassAbilityEffect> Effects { get; private set; } = new();

    public ClassAbilityDefinition(string name, string description, int minLevel, string classOwner,
        IEnumerable<ClassAbilityEffect>? effects = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ability name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Ability description is required.", nameof(description));
        if (minLevel < 1 || minLevel > 10)
            throw new ArgumentOutOfRangeException(nameof(minLevel), "MinLevel must be 1–10.");
        if (string.IsNullOrWhiteSpace(classOwner))
            throw new ArgumentException("ClassOwner is required.", nameof(classOwner));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        MinLevel = minLevel;
        ClassOwner = classOwner;
        Effects = effects?.ToList() ?? new List<ClassAbilityEffect>();
    }

    public void SetEffects(IEnumerable<ClassAbilityEffect> effects)
    {
        Effects = effects.ToList();
    }

    private ClassAbilityDefinition() { } // EF Core
}
