namespace WWN.Domain.Lookups;

/// <summary>
/// A lookup-table row: a stable id, a stable string code, a human-facing display name,
/// and optional metadata (description, sort order). Both enum-backed catalogs and
/// future DB-backed reference tables expose this shape.
/// </summary>
public interface ILookupValue
{
    int Id { get; }
    string Code { get; }
    string DisplayName { get; }
    string? Description { get; }
    int SortOrder { get; }
}
