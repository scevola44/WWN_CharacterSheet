using WWN.Application.DTOs;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.Rules;

namespace WWN.Application.Services;

public class CharacterArtService(
    ICharacterRepository characterRepository,
    IArtRepository artRepository,
    CharacterDetailMapper mapper)
{
    public async Task<KnownArtDto> LearnArtAsync(
        Guid characterId,
        string userId,
        Guid artId,
        CancellationToken cancellationToken = default)
    {
        var art = await GetArtOrThrow(artId, cancellationToken);
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);

        var knownArt = new KnownArt(artId);
        character.LearnArt(knownArt);
        await characterRepository.UpdateAsync(character, cancellationToken);

        return new KnownArtDto
        {
            Id = knownArt.Id,
            ArtId = artId,
            Art = ArtService.MapToDto(art)
        };
    }

    public async Task ForgetArtAsync(
        Guid characterId,
        string userId,
        Guid artId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.ForgetArt(artId);
        await characterRepository.UpdateAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> CommitEffortAsync(
        Guid characterId,
        string userId,
        int commitment,
        int amount,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        var kind = (EffortCommitment)commitment;
        if (!Enum.IsDefined(kind))
            throw new ArgumentException(
                $"'{commitment}' is not a valid EffortCommitment id.",
                nameof(commitment));
        var max = EffortPoolCalculator.CalculateMax(character);
        character.CommitEffort(kind, max, amount);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.MapToDetailDtoAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> EndSceneAsync(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.EndScene();
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.MapToDetailDtoAsync(character, cancellationToken);
    }

    public Task<CharacterDetailDto> RestForDayAsync(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return characterRepository.ExecuteInTransactionAsync(async () =>
        {
            var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
            character.RestForDay();
            await characterRepository.UpdateAsync(character, cancellationToken);
            return await mapper.MapToDetailDtoAsync(character, cancellationToken);
        }, cancellationToken);
    }

    public async Task<CharacterDetailDto> ReleaseSustainedAsync(
        Guid characterId,
        string userId,
        int amount,
        CancellationToken cancellationToken = default)
    {
        var character = await GetCharacterOrThrow(characterId, userId, cancellationToken);
        character.ReleaseSustainedEffort(amount);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.MapToDetailDtoAsync(character, cancellationToken);
    }

    private async Task<Domain.Aggregates.Character> GetCharacterOrThrow(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await characterRepository.GetByIdAsync(characterId, userId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
    }

    private async Task<Art> GetArtOrThrow(Guid artId, CancellationToken cancellationToken)
    {
        return await artRepository.GetByIdAsync(artId, cancellationToken)
            ?? throw new KeyNotFoundException($"Art {artId} not found.");
    }
}
