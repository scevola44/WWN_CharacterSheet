using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class SpellService(ISpellRepository spellRepository)
{
    public async Task<IReadOnlyList<SpellDto>> ListSpellsAsync(CancellationToken cancellationToken = default)
    {
        var spells = await spellRepository.GetAllAsync(cancellationToken);
        return spells.Select(MapToDto).ToList();
    }

    public async Task<SpellDto?> GetSpellAsync(
        Guid spellId, 
        CancellationToken cancellationToken = default)
    {
        var spell = await spellRepository.GetByIdAsync(spellId, cancellationToken);
        return spell is null ? null : MapToDto(spell);
    }

    public async Task<SpellDto> CreateSpellAsync(
        CreateSpellRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spell = new Spell(request.Name, request.SpellLevel, request.Description, request.Summary);
        await spellRepository.AddAsync(spell, cancellationToken);
        return MapToDto(spell);
    }

    public async Task<SpellDto?> UpdateSpellAsync(
        Guid spellId, 
        UpdateSpellRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spell = await spellRepository.GetByIdAsync(spellId, cancellationToken);
        if (spell is null)
            return null;

        spell.Update(request.Name, request.SpellLevel, request.Summary, request.Description);
        await spellRepository.UpdateAsync(spell, cancellationToken);
        return MapToDto(spell);
    }

    public async Task DeleteSpellAsync(
        Guid spellId, 
        CancellationToken cancellationToken = default)
    {
        await spellRepository.DeleteAsync(spellId, cancellationToken);
    }

    internal static SpellDto MapToDto(Spell spell)
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
