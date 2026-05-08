namespace WWN.Application.DTOs;

public record LookupValueDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int SortOrder { get; init; }
}

public record LookupsDto
{
    public IReadOnlyList<LookupValueDto> EffortCommitment { get; init; } = Array.Empty<LookupValueDto>();
}
