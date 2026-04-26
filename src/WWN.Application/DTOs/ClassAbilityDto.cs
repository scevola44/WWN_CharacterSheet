namespace WWN.Application.DTOs;

public record ClassAbilityDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int MinLevel { get; init; }
    public string ClassOwner { get; init; } = string.Empty;
}
