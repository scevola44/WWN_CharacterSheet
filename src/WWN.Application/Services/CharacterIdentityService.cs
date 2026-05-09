using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Aggregates;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

public class CharacterIdentityService(
    ICharacterRepository characterRepository,
    CharacterDetailMapper mapper)
{
    public async Task<CharacterDetailDto?> GetCharacterAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var character = await characterRepository.GetByIdAsync(id, userId, cancellationToken);
        return character is null ? null : await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<IReadOnlyList<CharacterSummaryDto>> ListCharactersAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var characters = await characterRepository.GetAllSummariesAsync(userId, cancellationToken);
        return characters.Select(c => new CharacterSummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            Class = c.Class.ToString(),
            Level = c.Level,
            CurrentHitPoints = c.CurrentHitPoints,
            MaxHitPoints = c.MaxHitPoints
        }).ToList();
    }

    public async Task<Guid> CreateCharacterAsync(
        CreateCharacterRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var characterClass = EnumParser.Parse<CharacterClass>(request.Class, nameof(request.Class));
        PartialClass? partialClassA = request.PartialClassA is not null
            ? EnumParser.Parse<PartialClass>(request.PartialClassA, nameof(request.PartialClassA))
            : null;
        PartialClass? partialClassB = request.PartialClassB is not null
            ? EnumParser.Parse<PartialClass>(request.PartialClassB, nameof(request.PartialClassB))
            : null;

        var attributeScores = request.Attributes.ToDictionary(
            kvp => EnumParser.Parse<AttributeName>(kvp.Key, nameof(request.Attributes)),
            kvp => kvp.Value);

        var createdCharacter = Character.Create(request.Name, characterClass, attributeScores,
            userId, request.Background, request.Origin, partialClassA, partialClassB, request.MaxHitPoints, request.Level);

        await characterRepository.AddAsync(createdCharacter, cancellationToken);
        return createdCharacter.Id;
    }

    public async Task<CharacterDetailDto> UpdateAttributeAsync(
        Guid characterId,
        string userId,
        string attributeString,
        int score,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var attributeName = EnumParser.Parse<AttributeName>(attributeString, nameof(attributeString));
        character.SetAttribute(attributeName, score);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateSkillAsync(
        Guid charId,
        string userId,
        string skillName,
        int rank,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(charId, userId, cancellationToken);
        var skill = EnumParser.Parse<SkillName>(skillName, nameof(skillName));
        character.SetSkillRank(skill, rank);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> AddCustomSkillAsync(
        Guid characterId,
        string userId,
        string name,
        int rank,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.AddCustomSkill(name, rank);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetHpAsync(
        Guid characterId,
        string userId,
        int maxHp,
        int currentHp,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetHitPoints(maxHp, currentHp);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetStrainAsync(
        Guid characterId,
        string userId,
        int currentStrain,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetStrain(currentStrain);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetLevelAsync(
        Guid characterId,
        string userId,
        int level,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetLevel(level);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public Task<CharacterDetailDto> LevelUpAsync(
        Guid characterId,
        string userId,
        int hpGain,
        CancellationToken cancellationToken = default)
    {
        return characterRepository.ExecuteInTransactionAsync(async () =>
        {
            var character = await GetOrThrow(characterId, userId, cancellationToken);
            character.LevelUp(hpGain);
            await characterRepository.UpdateAsync(character, cancellationToken);
            return await mapper.SyncAndMapAsync(character, cancellationToken);
        }, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateNotesAsync(
        Guid characterId,
        string userId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetNotes(notes);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task DeleteCharacterAsync(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        await characterRepository.DeleteAsync(characterId, userId, cancellationToken);
    }

    private async Task<Character> GetOrThrow(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await characterRepository.GetByIdAsync(characterId, userId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
    }
}
