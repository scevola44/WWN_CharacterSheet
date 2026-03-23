using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

// TODO since we have the int Level property, it would be best practice to not hard-code logic to manually consider levels 1 and 2, but to be more abstract.
public class Focus
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Level { get; private set; }
    public List<FocusEffect> Effects { get; private set; } = new();
    public bool ConditionalActive { get; private set; }

    public Focus(string name, int level, IEnumerable<FocusEffect> effects)
    {
        if (level is < 1 or > 2)
            throw new ArgumentOutOfRangeException(nameof(level), level, "Focus level must be 1 or 2.");
        Id = Guid.NewGuid();
        Name = name;
        Level = level;
        Effects = effects.ToList();
    }

    private Focus() { } // EF Core

    public void UpgradeToLevel2(IEnumerable<FocusEffect> additionalEffects)
    {
        if (Level >= 2)
            throw new InvalidOperationException("Focus is already at max level.");
        Level = 2;
        Effects.AddRange(additionalEffects);
    }

    public void SetConditionalActive(bool active) => ConditionalActive = active;

    public void SetEffects(IEnumerable<FocusEffect> effects)
    {
        Effects = effects.ToList();
    }
}
