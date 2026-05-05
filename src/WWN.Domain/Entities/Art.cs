using WWN.Domain.Enums;

namespace WWN.Domain.Entities;

public class Art
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public int MinLevel { get; private set; }
    public EffortCommitment? EffortCost { get; private set; }
    public string Source { get; private set; } = string.Empty;

    public Art(string name, string description, int minLevel, string source,
        EffortCommitment? effortCost = null, string? summary = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Art name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Art description is required.", nameof(description));
        if (minLevel is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(minLevel), "MinLevel must be 1-10.");
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Source is required.", nameof(source));

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        MinLevel = minLevel;
        Source = source;
        EffortCost = effortCost;
        Summary = summary;
    }

    public void Update(string name, string description, int minLevel, string source,
        EffortCommitment? effortCost, string? summary)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Art name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Art description is required.", nameof(description));
        if (minLevel is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(minLevel), "MinLevel must be 1-10.");
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Source is required.", nameof(source));

        Name = name;
        Description = description;
        MinLevel = minLevel;
        Source = source;
        EffortCost = effortCost;
        Summary = summary;
    }

    private Art() { } // EF Core
}
