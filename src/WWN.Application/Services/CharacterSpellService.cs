using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class CharacterSpellService(
    ICharacterRepository characterRepository,
    ISpellRepository spellRepository,
    CharacterService characterService)
{
    public async Task<KnownSpellDto> LearnSpellAsync(
        Guid characterId,
        string userId,
        Guid spellId,
        CancellationToken cancellationToken = default)
    {
        var spell = await GetSpellOrThrow(spellId, cancellationToken);
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);

        var knownSpell = new KnownSpell(spellId);
        character.LearnSpell(knownSpell);
        await characterRepository.UpdateAsync(character, cancellationToken);

        return new KnownSpellDto
        {
            Id = knownSpell.Id,
            SpellId = spellId,
            Spell = SpellService.MapToDto(spell)
        };
    }

    public async Task ForgetSpellAsync(
        Guid characterId,
        string userId,
        Guid spellId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.ForgetSpell(spellId);
        await characterRepository.UpdateAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UseSpellSlotAsync(
        Guid characterId,
        string userId,
        int spellLevel,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.UseSpellSlot(spellLevel);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await characterService.MapToDetailDtoAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> RestoreSpellSlotsAsync(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.RestoreAllSpellSlots();
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await characterService.MapToDetailDtoAsync(character, cancellationToken);
    }

    private async Task<Domain.Aggregates.Character> GetCharacterOrThrow(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await characterRepository.GetByIdAsync(characterId, userId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
    }

    private async Task<Spell> GetSpellOrThrow(Guid spellId,
        CancellationToken cancellationToken)
    {
        return await spellRepository.GetByIdAsync(spellId, cancellationToken)
            ?? throw new KeyNotFoundException($"Spell {spellId} not found.");
    }
}
