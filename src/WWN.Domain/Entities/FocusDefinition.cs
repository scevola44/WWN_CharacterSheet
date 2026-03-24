using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

public class FocusDefinition
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Level1Description { get; private set; } = string.Empty;
    public string? Level2Description { get; private set; }
    public bool HasLevel2 { get; private set; }
    public bool CanTakeMultipleTimes { get; private set; }
    public List<FocusEffect> Level1Effects { get; private set; } = new();
    public List<FocusEffect> Level2Effects { get; private set; } = new();

    public FocusDefinition(
        string name,
        string level1Description,
        string? level2Description = null,
        string? description = null,
        bool canTakeMultipleTimes = false,
        IEnumerable<FocusEffect>? level1Effects = null,
        IEnumerable<FocusEffect>? level2Effects = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Focus name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(level1Description))
            throw new ArgumentException("Level 1 description is required.", nameof(level1Description));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Level1Description = level1Description;
        Level2Description = level2Description;
        HasLevel2 = level2Description is not null;
        CanTakeMultipleTimes = canTakeMultipleTimes;
        Level1Effects = level1Effects?.ToList() ?? new();
        Level2Effects = level2Effects?.ToList() ?? new();
    }

    public void Update(
        string name,
        string? description,
        string level1Description,
        string? level2Description,
        bool canTakeMultipleTimes,
        IEnumerable<FocusEffect>? level1Effects = null,
        IEnumerable<FocusEffect>? level2Effects = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Focus name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(level1Description))
            throw new ArgumentException("Level 1 description is required.", nameof(level1Description));

        Name = name;
        Description = description;
        Level1Description = level1Description;
        Level2Description = level2Description;
        HasLevel2 = level2Description is not null;
        CanTakeMultipleTimes = canTakeMultipleTimes;
        Level1Effects = level1Effects?.ToList() ?? Level1Effects;
        Level2Effects = level2Effects?.ToList() ?? Level2Effects;
    }

    private FocusDefinition() { } // EF Core
}
