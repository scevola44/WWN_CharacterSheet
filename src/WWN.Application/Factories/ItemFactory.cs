using WWN.Application.DTOs;
using WWN.Application.Helpers;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Factories;

public static class ItemFactory
{
    public static Item Create(AddItemRequest req)
    {
        var slotType = EnumParser.Parse<ItemSlotType>(req.SlotType, nameof(req.SlotType));

        return req.ItemType.ToLower() switch
        {
            "weapon" => new Weapon(
                req.Name,
                req.Encumbrance,
                new DamageDie(req.DamageDieCount ?? 1, req.DamageDieSides ?? 6),
                EnumParser.Parse<AttributeName>(req.AttributeModifier ?? "Strength", nameof(req.AttributeModifier)),
                ParseWeaponTags(req.Tags),
                req.ShockDamage.HasValue && req.ShockAcThreshold.HasValue
                    ? new ShockInfo(req.ShockDamage.Value, req.ShockAcThreshold.Value)
                    : null,
                slotType,
                req.Description),
            "armor" => new Armor(
                req.Name,
                req.Encumbrance,
                req.AcBonus ?? 0,
                req.IsShield ?? false,
                slotType,
                req.Description),
            _ => new Item(req.Name, req.Encumbrance, slotType, req.Quantity, req.Description)
        };
    }

    private static WeaponTag ParseWeaponTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags)) return WeaponTag.None;
        return EnumParser.Parse<WeaponTag>(tags, nameof(tags));
    }
}
