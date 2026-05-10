namespace WWN.Domain.Entities;

public class ArtSource
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }

    public ArtSource(string code, string displayName, string? description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName is required.", nameof(displayName));

        Code = code;
        DisplayName = displayName;
        Description = description;
        SortOrder = sortOrder;
    }

    public void Update(string displayName, string? description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("DisplayName is required.", nameof(displayName));

        DisplayName = displayName;
        Description = description;
        SortOrder = sortOrder;
    }

    private ArtSource() { } // EF Core
}
