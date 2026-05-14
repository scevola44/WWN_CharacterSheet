namespace WWN.Application.DTOs;

public record LookupValueDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int SortOrder { get; init; }
    public string? Abbreviation { get; init; }
}

public record LookupsDto
{
    public IReadOnlyList<LookupValueDto> EffortCommitment { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> ArtSources { get; init; } = Array.Empty<LookupValueDto>();
    // Phase 1
    public IReadOnlyList<LookupValueDto> SkillNames { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> AttributeNames { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> ItemSlotTypes { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> SaveTypes { get; init; } = Array.Empty<LookupValueDto>();
    // Phase 2
    public IReadOnlyList<LookupValueDto> CharacterClasses { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> PartialClasses { get; init; } = Array.Empty<LookupValueDto>();
    public IReadOnlyList<LookupValueDto> FocusEffectTypes { get; init; } = Array.Empty<LookupValueDto>();
    // Phase 3
    public IReadOnlyList<LookupValueDto> WeaponTags { get; init; } = Array.Empty<LookupValueDto>();
}
