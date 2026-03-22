using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class SpellService
{
    private readonly ISpellRepository _repo;

    public SpellService(ISpellRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<SpellDto>> ListSpellsAsync(CancellationToken ct = default)
    {
        var spells = await _repo.GetAllAsync(ct);
        return spells.Select(MapToDto).ToList();
    }

    public async Task<SpellDto?> GetSpellAsync(Guid id, CancellationToken ct = default)
    {
        var spell = await _repo.GetByIdAsync(id, ct);
        return spell is null ? null : MapToDto(spell);
    }

    public async Task<SpellDto> CreateSpellAsync(CreateSpellRequest req, CancellationToken ct = default)
    {
        var spell = new Spell(req.Name, req.SpellLevel, req.Description, req.School, req.Duration, req.Range);
        await _repo.AddAsync(spell, ct);
        return MapToDto(spell);
    }

    public async Task DeleteSpellAsync(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
    }

    private SpellDto MapToDto(Spell spell)
    {
        return new SpellDto
        {
            Id = spell.Id,
            Name = spell.Name,
            SpellLevel = spell.SpellLevel,
            Description = spell.Description,
            School = spell.School,
            Duration = spell.Duration,
            Range = spell.Range
        };
    }
}

public record CreateSpellRequest
{
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? School { get; init; }
    public string? Duration { get; init; }
    public string? Range { get; init; }
}
