namespace WWN.Domain.Lookups;

public sealed record LookupValue(
    int Id,
    string Code,
    string DisplayName,
    string? Description,
    int SortOrder
) : ILookupValue;
