using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class CharacterSpellService
{
    private readonly ICharacterRepository _charRepo;
    private readonly ISpellRepository _spellRepo;
    private readonly CharacterService _charService;

    public CharacterSpellService(ICharacterRepository charRepo, ISpellRepository spellRepo, CharacterService charService)
    {
        _charRepo = charRepo;
        _spellRepo = spellRepo;
        _charService = charService;
    }

    public async Task<KnownSpellDto> LearnSpellAsync(Guid charId, Guid spellId, CancellationToken ct = default)
    {
        var character = await GetCharacterOrThrow(charId, ct);
        var spell = await GetSpellOrThrow(spellId, ct);

        if (character.Spellbook.Any(s => s.SpellId == spellId))
            throw new InvalidOperationException("Character already knows this spell.");

        var knownSpell = new KnownSpell(spellId, spell);
        knownSpell.GetType().GetProperty(nameof(KnownSpell.CharacterId))
            ?.SetValue(knownSpell, charId);

        character.LearnSpell(knownSpell);
        await _charRepo.UpdateAsync(character, ct);

        return new KnownSpellDto
        {
            Id = knownSpell.Id,
            SpellId = spellId,
            Spell = new SpellDto
            {
                Id = spell.Id,
                Name = spell.Name,
                SpellLevel = spell.SpellLevel,
                Description = spell.Description,
                Summary = spell.Summary
            }
        };
    }

    public async Task ForgetSpellAsync(Guid charId, Guid spellId, CancellationToken ct = default)
    {
        var character = await GetCharacterOrThrow(charId, ct);
        character.ForgetSpell(spellId);
        await _charRepo.UpdateAsync(character, ct);
    }

    public async Task<CharacterDetailDto> UseSpellSlotAsync(Guid charId, int spellLevel, CancellationToken ct = default)
    {
        var character = await GetCharacterOrThrow(charId, ct);
        character.UseSpellSlot(spellLevel);
        await _charRepo.UpdateAsync(character, ct);
        return await _charService.GetCharacterAsync(charId, ct)
            ?? throw new KeyNotFoundException($"Character {charId} not found.");
    }

    public async Task<CharacterDetailDto> RestoreSpellSlotsAsync(Guid charId, CancellationToken ct = default)
    {
        var character = await GetCharacterOrThrow(charId, ct);
        character.RestoreAllSpellSlots();
        await _charRepo.UpdateAsync(character, ct);
        return await _charService.GetCharacterAsync(charId, ct)
            ?? throw new KeyNotFoundException($"Character {charId} not found.");
    }

    private async Task<Domain.Aggregates.Character> GetCharacterOrThrow(Guid id, CancellationToken ct)
    {
        return await _charRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Character {id} not found.");
    }

    private async Task<Spell> GetSpellOrThrow(Guid id, CancellationToken ct)
    {
        return await _spellRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Spell {id} not found.");
    }
}
