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
        var spell = new Spell(req.Name, req.SpellLevel, req.Description, req.Summary);
        await _repo.AddAsync(spell, ct);
        return MapToDto(spell);
    }

    public async Task<SpellDto?> UpdateSpellAsync(Guid id, UpdateSpellRequest req, CancellationToken ct = default)
    {
        var spell = await _repo.GetByIdAsync(id, ct);
        if (spell is null)
            return null;

        spell.Update(req.Name, req.SpellLevel, req.Summary, req.Description);
        await _repo.UpdateAsync(spell, ct);
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
            Summary = spell.Summary
        };
    }
}

public record CreateSpellRequest
{
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record UpdateSpellRequest
{
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}
