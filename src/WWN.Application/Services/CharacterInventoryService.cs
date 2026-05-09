using WWN.Application.DTOs;
using WWN.Application.Factories;
using WWN.Application.Helpers;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

public class CharacterInventoryService(
    ICharacterRepository characterRepository,
    CharacterDetailMapper mapper)
{
    public async Task<CharacterDetailDto> AddItemAsync(
        Guid characterId,
        string userId,
        AddItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var item = ItemFactory.Create(request);
        character.AddItem(item);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task RemoveItemAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.RemoveItem(itemId);
        await characterRepository.UpdateAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateItemAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        UpdateItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var item = character.Inventory.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            throw new InvalidOperationException($"Item with ID {itemId} not found in character inventory.");

        switch (request.ItemType.ToLower())
        {
            case "weapon":
                if (item is not Weapon weapon)
                    throw new InvalidOperationException($"Item {itemId} is not a weapon.");
                weapon.Update(
                    request.Name,
                    request.Encumbrance,
                    new DamageDie(request.DamageDieCount ?? 1, request.DamageDieSides ?? 6),
                    EnumParser.Parse<AttributeName>(request.AttributeModifier ?? "Strength", nameof(request.AttributeModifier)),
                    EnumParser.Parse<SkillName>(request.CombatSkill ?? "Stab", nameof(request.CombatSkill)),
                    ParseWeaponTags(request.Tags),
                    request is { ShockDamage: not null, ShockAcThreshold: not null }
                        ? new ShockInfo(request.ShockDamage.Value, request.ShockAcThreshold.Value)
                        : null,
                    request.Description);
                break;

            case "armor":
                if (item is not Armor armor)
                    throw new InvalidOperationException($"Item {itemId} is not armor.");
                armor.Update(
                    request.Name,
                    request.Encumbrance,
                    request.AcBonus ?? 0,
                    request.IsShield ?? false,
                    request.Description);
                break;

            default:
                item.Update(request.Name, request.Encumbrance, request.Quantity, request.Description);
                break;
        }

        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.MapToDetailDtoAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> ChangeSlotAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        string slotType,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var slot = EnumParser.Parse<ItemSlotType>(slotType, nameof(slotType));
        character.ChangeItemSlot(itemId, slot);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateWeaponAttackConfigAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        string skill,
        string attribute,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var weapon = character.Inventory.OfType<Weapon>().FirstOrDefault(w => w.Id == itemId);
        if (weapon is null)
            throw new InvalidOperationException($"Weapon with ID {itemId} not found in character inventory.");

        var skillName = EnumParser.Parse<SkillName>(skill, nameof(skill));
        var attrName = EnumParser.Parse<AttributeName>(attribute, nameof(attribute));
        weapon.SetCombatConfig(skillName, attrName);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await mapper.SyncAndMapAsync(character, cancellationToken);
    }

    private static WeaponTag ParseWeaponTags(string? tags)
    {
        return string.IsNullOrWhiteSpace(tags)
            ? WeaponTag.None
            : EnumParser.Parse<WeaponTag>(tags, nameof(tags));
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
