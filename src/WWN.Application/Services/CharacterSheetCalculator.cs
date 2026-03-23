using WWN.Application.DTOs;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Application.Services;

public class CharacterSheetCalculator
{
    public static DerivedStatsDto Calculate(Character character)
    {
        return new DerivedStatsDto
        {
            ArmorClass = CombatCalculator.GetArmorClass(character),
            BaseAttackBonus = CombatCalculator.GetBaseAttackBonus(
                character.Class, character.PartialClassA, character.PartialClassB, character.Level),
            PhysicalSave = SavingThrowCalculator.GetSaveTarget(SaveType.Physical, character, false),
            EvasionSave = SavingThrowCalculator.GetSaveTarget(SaveType.Evasion, character, false),
            MentalSave = SavingThrowCalculator.GetSaveTarget(SaveType.Mental, character, false),
            AttributeModifiers = Enum.GetValues<AttributeName>()
                .ToDictionary(a => a.ToString(), a => character.GetAttribute(a).Modifier),
            WeaponAttackBonuses = character.Inventory
                .OfType<Weapon>()
                .Where(w => w.SlotType == ItemSlotType.Equipped)
                .ToDictionary(w => w.Id, w => CombatCalculator.GetTotalAttackBonus(character, w)),
            WeaponDamageBonuses = character.Inventory
                .OfType<Weapon>()
                .Where(w => w.SlotType == ItemSlotType.Equipped)
                .ToDictionary(w => w.Id, w => CombatCalculator.GetTotalDamageBonus(character, w)),
            HitDieModifier = HitPointCalculator.GetHitDieModifier(
                character.Class, character.PartialClassA, character.PartialClassB)
        };
    }
}
