using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

public class CharacterFocusService(
    ICharacterRepository characterRepository,
    CharacterDetailMapper mapper)
{
    public async Task<CharacterDetailDto> AddFocusAsync(
        Guid characterId,
        string userId,
        AddFocusRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focusEffects = request.Effects.Select(e => new FocusEffect(
            EnumParser.Parse<FocusEffectType>(e.Type, nameof(e.Type)),
            e.NumericValue,
            EnumParser.Parse<FocusEffectValueType>(e.ValueType, nameof(e.ValueType)),
            EnumParser.Parse<FocusEffectCondition>(e.Condition, nameof(e.Condition)),
            e.TargetSkill is not null ? EnumParser.Parse<SkillName>(e.TargetSkill, nameof(e.TargetSkill)) : null,
            e.TargetAttribute is not null ? EnumParser.Parse<AttributeName>(e.TargetAttribute, nameof(e.TargetAttribute)) : null,
            e.Description));

        var focus = new Focus(request.Name, request.Level, focusEffects);
        character.AddFocus(focus);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpgradeFocusAsync(
        Guid characterId,
        string userId,
        Guid focusId,
        UpgradeFocusRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focus = character.Foci.FirstOrDefault(f => f.Id == focusId)
            ?? throw new KeyNotFoundException($"Focus {focusId} not found on character {characterId}.");
        focus.UpgradeToLevel2(request.AdditionalEffects.Select(ParseFocusEffect));
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetFocusConditionalAsync(
        Guid characterId,
        string userId,
        Guid focusId,
        bool active,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focus = character.Foci.FirstOrDefault(f => f.Id == focusId)
            ?? throw new KeyNotFoundException($"Focus {focusId} not found on character {characterId}.");
        focus.SetConditionalActive(active);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task RemoveFocusAsync(Guid characterId, string userId, Guid focusId, CancellationToken ct = default)
    {
        var character = await GetOrThrow(characterId, userId, ct);
        character.RemoveFocus(focusId);
        await characterRepository.UpdateAsync(character, ct);
    }

    private async Task<Character> GetOrThrow(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await characterRepository.GetByIdAsync(characterId, userId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
    }

    private static FocusEffect ParseFocusEffect(FocusEffectDto e) => new(
        EnumParser.Parse<FocusEffectType>(e.Type, nameof(e.Type)),
        e.NumericValue,
        EnumParser.ParseOrDefault(e.ValueType, FocusEffectValueType.Static),
        EnumParser.ParseOrDefault(e.Condition, FocusEffectCondition.Always),
        e.TargetSkill is not null ? EnumParser.Parse<SkillName>(e.TargetSkill, nameof(e.TargetSkill)) : null,
        e.TargetAttribute is not null ? EnumParser.Parse<AttributeName>(e.TargetAttribute, nameof(e.TargetAttribute)) : null,
        e.Description);
}
