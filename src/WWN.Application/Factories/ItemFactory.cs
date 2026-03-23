using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Factories;

public static class ItemFactory
{
    public static Item Create(AddItemRequest request)
    {
        var slotType = EnumParser.Parse<ItemSlotType>(request.SlotType, nameof(request.SlotType));

        return request.ItemType.ToLower() switch
        {
            "weapon" => new Weapon(
                request.Name,
                request.Encumbrance,
                new DamageDie(request.DamageDieCount ?? 1, request.DamageDieSides ?? 6),
                EnumParser.Parse<AttributeName>(request.AttributeModifier ?? "Strength", nameof(request.AttributeModifier)),
                ParseWeaponTags(request.Tags),
                request is { ShockDamage: not null, ShockAcThreshold: not null }
                    ? new ShockInfo(request.ShockDamage.Value, request.ShockAcThreshold.Value)
                    : null,
                slotType,
                request.Description),
            "armor" => new Armor(
                request.Name,
                request.Encumbrance,
                request.AcBonus ?? 0,
                request.IsShield ?? false,
                slotType,
                request.Description),
            _ => new Item(request.Name, request.Encumbrance, slotType, request.Quantity, request.Description)
        };
    }

    private static WeaponTag ParseWeaponTags(string? tags)
    {
        return string.IsNullOrWhiteSpace(tags) 
            ? WeaponTag.None 
            : EnumParser.Parse<WeaponTag>(tags, nameof(tags));
    }
}
