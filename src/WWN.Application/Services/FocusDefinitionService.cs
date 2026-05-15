using Microsoft.Extensions.Caching.Memory;
using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

public class FocusDefinitionService(IFocusDefinitionRepository focusDefinitionRepository, IMemoryCache cache)
{
    public async Task<IReadOnlyList<FocusDefinitionDto>> ListAsync(
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var foci = await focusDefinitionRepository.GetVisibleToUserAsync(userId, cancellationToken);
        return foci.Select(MapToDto).ToList();
    }

    public async Task<FocusDefinitionDto?> GetAsync(
        Guid focusId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = await focusDefinitionRepository.GetByIdAsync(focusId, cancellationToken);
        if (focusDefinition is null) return null;
        if (focusDefinition.OwnerId is not null && focusDefinition.OwnerId != userId) return null;
        return MapToDto(focusDefinition);
    }

    public async Task<FocusDefinitionDto> CreateAsync(
        CreateFocusDefinitionRequest request,
        string? ownerId = null,
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = new FocusDefinition(
            request.Name,
            request.Level1Description,
            request.Level2Description,
            request.Description,
            request.CanTakeMultipleTimes,
            request.Level1Effects.Select(ParseEffect),
            request.Level2Effects.Select(ParseEffect),
            ownerId);
        await focusDefinitionRepository.AddAsync(focusDefinition, cancellationToken);
        cache.Remove(CharacterDetailMapper.FocusDefsKey);
        return MapToDto(focusDefinition);
    }

    public async Task<FocusDefinitionDto?> UpdateAsync(
        Guid focusId,
        UpdateFocusDefinitionRequest request,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = await focusDefinitionRepository.GetByIdAsync(focusId, cancellationToken);
        if (focusDefinition is null) return null;
        EnsureCanMutate(focusDefinition.OwnerId, userId, isAdmin);

        focusDefinition.Update(
            request.Name,
            request.Description,
            request.Level1Description,
            request.Level2Description,
            request.CanTakeMultipleTimes,
            request.Level1Effects.Select(ParseEffect),
            request.Level2Effects.Select(ParseEffect));
        await focusDefinitionRepository.UpdateAsync(focusDefinition, cancellationToken);
        cache.Remove(CharacterDetailMapper.FocusDefsKey);
        return MapToDto(focusDefinition);
    }

    public async Task DeleteAsync(
        Guid focusId,
        string? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var focusDefinition = await focusDefinitionRepository.GetByIdAsync(focusId, cancellationToken);
        if (focusDefinition is null) return;
        EnsureCanMutate(focusDefinition.OwnerId, userId, isAdmin);
        await focusDefinitionRepository.DeleteAsync(focusId, cancellationToken);
        cache.Remove(CharacterDetailMapper.FocusDefsKey);
    }

    private static void EnsureCanMutate(string? ownerId, string? userId, bool isAdmin)
    {
        if (isAdmin) return;
        if (ownerId is null)
            throw new UnauthorizedAccessException("Only an admin can modify built-in foci.");
        if (ownerId != userId)
            throw new UnauthorizedAccessException("You can only modify foci you created.");
    }

    private static FocusEffect ParseEffect(FocusEffectDto e) => new(
        EnumParser.Parse<FocusEffectType>(e.Type, nameof(e.Type)),
        e.NumericValue,
        EnumParser.ParseOrDefault(e.ValueType, FocusEffectValueType.Static),
        EnumParser.ParseOrDefault(e.Condition, FocusEffectCondition.Always),
        e.TargetSkill is not null ? EnumParser.Parse<SkillName>(e.TargetSkill, nameof(e.TargetSkill)) : null,
        e.TargetAttribute is not null ? EnumParser.Parse<AttributeName>(e.TargetAttribute, nameof(e.TargetAttribute)) : null,
        e.Description);

    private static FocusEffectDto MapEffectDto(FocusEffect e) => new()
    {
        Type = e.Type.ToString(),
        NumericValue = e.NumericValue,
        ValueType = e.ValueType.ToString(),
        Condition = e.Condition.ToString(),
        TargetSkill = e.TargetSkill?.ToString(),
        TargetAttribute = e.TargetAttribute?.ToString(),
        Description = e.Description
    };

    private static FocusDefinitionDto MapToDto(FocusDefinition fd) => new()
    {
        Id = fd.Id,
        Name = fd.Name,
        Description = fd.Description,
        Level1Description = fd.Level1Description,
        Level2Description = fd.Level2Description,
        HasLevel2 = fd.HasLevel2,
        CanTakeMultipleTimes = fd.CanTakeMultipleTimes,
        Level1Effects = fd.Level1Effects.Select(MapEffectDto).ToList(),
        Level2Effects = fd.Level2Effects.Select(MapEffectDto).ToList(),
        IsCustom = fd.OwnerId is not null,
    };
}
