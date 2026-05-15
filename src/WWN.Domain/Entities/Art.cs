using WWN.Domain.Enums;

namespace WWN.Domain.Entities;

public class Art
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public int MinLevel { get; private set; }
    public EffortCommitment EffortCost { get; private set; }
    public int SourceId { get; private set; }
    public ArtSource? SourceNavigation { get; private set; } // EF Core navigation
    public string? OwnerId { get; private set; }

    public Art(string name, string description, int minLevel, int sourceId,
        EffortCommitment effortCost = EffortCommitment.None, string? summary = null, string? ownerId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Art name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Art description is required.", nameof(description));
        if (minLevel is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(minLevel), "MinLevel must be 1-10.");
        if (sourceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(sourceId), "SourceId must be positive.");
        if (!Enum.IsDefined(effortCost))
            throw new ArgumentOutOfRangeException(nameof(effortCost), effortCost, "Unknown EffortCommitment.");

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        MinLevel = minLevel;
        SourceId = sourceId;
        EffortCost = effortCost;
        Summary = summary;
        OwnerId = string.IsNullOrWhiteSpace(ownerId) ? null : ownerId;
    }

    public void Update(string name, string description, int minLevel, int sourceId,
        EffortCommitment effortCost, string? summary)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Art name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Art description is required.", nameof(description));
        if (minLevel is < 1 or > 10)
            throw new ArgumentOutOfRangeException(nameof(minLevel), "MinLevel must be 1-10.");
        if (sourceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(sourceId), "SourceId must be positive.");
        if (!Enum.IsDefined(effortCost))
            throw new ArgumentOutOfRangeException(nameof(effortCost), effortCost, "Unknown EffortCommitment.");

        Name = name;
        Description = description;
        MinLevel = minLevel;
        SourceId = sourceId;
        EffortCost = effortCost;
        Summary = summary;
    }

    private Art() { } // EF Core
}
