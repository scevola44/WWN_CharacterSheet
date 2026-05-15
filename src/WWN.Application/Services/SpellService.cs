using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class SpellService(ISpellRepository spellRepository)
{
    public async Task<IReadOnlyList<SpellDto>> ListSpellsAsync(
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var spells = await spellRepository.GetVisibleToUserAsync(userId, cancellationToken);
        return spells.Select(MapToDto).ToList();
    }

    public async Task<SpellDto?> GetSpellAsync(
        Guid spellId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var spell = await spellRepository.GetByIdAsync(spellId, cancellationToken);
        if (spell is null) return null;
        if (spell.OwnerId is not null && spell.OwnerId != userId) return null;
        return MapToDto(spell);
    }

    public async Task<SpellDto> CreateSpellAsync(
        CreateSpellRequest request,
        string? ownerId = null,
        CancellationToken cancellationToken = default)
    {
        var spell = new Spell(request.Name, request.SpellLevel, request.Description, request.Summary, ownerId);
        await spellRepository.AddAsync(spell, cancellationToken);
        return MapToDto(spell);
    }

    public async Task<SpellDto?> UpdateSpellAsync(
        Guid spellId,
        UpdateSpellRequest request,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var spell = await spellRepository.GetByIdAsync(spellId, cancellationToken);
        if (spell is null) return null;
        EnsureCanMutate(spell.OwnerId, userId, isAdmin);

        spell.Update(request.Name, request.SpellLevel, request.Summary, request.Description);
        await spellRepository.UpdateAsync(spell, cancellationToken);
        return MapToDto(spell);
    }

    public async Task DeleteSpellAsync(
        Guid spellId,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var spell = await spellRepository.GetByIdAsync(spellId, cancellationToken);
        if (spell is null) return;
        EnsureCanMutate(spell.OwnerId, userId, isAdmin);
        await spellRepository.DeleteAsync(spellId, cancellationToken);
    }

    private static void EnsureCanMutate(string? ownerId, string? userId, bool isAdmin)
    {
        if (isAdmin) return;
        if (ownerId is null)
            throw new UnauthorizedAccessException("Only an admin can modify built-in spells.");
        if (ownerId != userId)
            throw new UnauthorizedAccessException("You can only modify spells you created.");
    }

    internal static SpellDto MapToDto(Spell spell)
    {
        return new SpellDto
        {
            Id = spell.Id,
            Name = spell.Name,
            SpellLevel = spell.SpellLevel,
            Description = spell.Description,
            Summary = spell.Summary,
            IsCustom = spell.OwnerId is not null
        };
    }
}
