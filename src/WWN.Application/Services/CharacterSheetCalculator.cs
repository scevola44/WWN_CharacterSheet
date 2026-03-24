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
        int saveFocusBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.SaveBonus, character);
        int hpFocusBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.HpBonus, character);
        int saveAbilityBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.SaveBonus, character);
        int hpAbilityBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.HpBonus, character);

        return new DerivedStatsDto
        {
            ArmorClass = CombatCalculator.GetArmorClass(character),
            BaseAttackBonus = CombatCalculator.GetBaseAttackBonus(
                character.Class, character.PartialClassA, character.PartialClassB, character.Level),
            PhysicalSave = SavingThrowCalculator.GetSaveTarget(SaveType.Physical, character, false) - saveFocusBonus - saveAbilityBonus,
            EvasionSave = SavingThrowCalculator.GetSaveTarget(SaveType.Evasion, character, false) - saveFocusBonus - saveAbilityBonus,
            MentalSave = SavingThrowCalculator.GetSaveTarget(SaveType.Mental, character, false) - saveFocusBonus - saveAbilityBonus,
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
            WeaponShockBonuses = character.Inventory
                .OfType<Weapon>()
                .Where(w => w.SlotType == ItemSlotType.Equipped && w.Shock != null)
                .ToDictionary(w => w.Id, w => CombatCalculator.GetTotalShockBonus(character, w)),
            HitDieModifier = HitPointCalculator.GetHitDieModifier(
                character.Class, character.PartialClassA, character.PartialClassB),
            HpFocusBonus = hpFocusBonus + hpAbilityBonus
        };
    }
}
