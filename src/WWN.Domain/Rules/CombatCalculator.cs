using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Rules;

public static class CombatCalculator
{
    public static int GetBaseAttackBonus(CharacterClass charClass, PartialClass? partialA,
        PartialClass? partialB, int level)
    {
        if (charClass == CharacterClass.Warrior)
            return level;

        bool hasPartialWarrior = partialA == PartialClass.PartialWarrior ||
                                 partialB == PartialClass.PartialWarrior;

        if (charClass == CharacterClass.Adventurer && hasPartialWarrior)
            return level / 2 + 1; // partial warriors get a slight edge

        return level / 2;
    }

    public static int GetTotalAttackBonus(Character character, Weapon weapon)
    {
        int bab = GetBaseAttackBonus(character.Class, character.PartialClassA,
            character.PartialClassB, character.Level);

        var skill = character.GetSkillOrDefault(weapon.CombatSkill);
        int skillLevel = skill?.Rank.Level ?? -1;
        int attrMod = character.GetAttribute(weapon.AttributeModifier).Modifier;
        var condition = MapSkillToCondition(weapon.CombatSkill);
        int focusBonus = FocusEffectAggregator.SumEffects(character.Foci, FocusEffectType.AttackBonus, character, condition);
        int abilityBonus = ClassAbilityEffectAggregator.SumEffects(character.ClassAbilities, FocusEffectType.AttackBonus, character, condition);

        return bab + skillLevel + attrMod + focusBonus + abilityBonus;
    }

    public static int GetTotalDamageBonus(Character character, Weapon weapon)
    {
        int attrMod = character.GetAttribute(weapon.AttributeModifier).Modifier;
        int focusBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.DamageBonus, character, MapSkillToCondition(weapon.CombatSkill));
        int abilityBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.DamageBonus, character, MapSkillToCondition(weapon.CombatSkill));

        return attrMod + focusBonus + abilityBonus;
    }

    public static int GetTotalShockBonus(Character character, Weapon weapon)
    {
        var condition = MapSkillToCondition(weapon.CombatSkill);
        int focusDmgBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.DamageBonus, character, condition);
        int abilityDmgBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.DamageBonus, character, condition);
        int focusShockBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.ShockBonus, character, condition);
        int abilityShockBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.ShockBonus, character, condition);

        return focusDmgBonus + abilityDmgBonus + focusShockBonus + abilityShockBonus;
    }

    public static int GetArmorClass(Character character)
    {
        int baseAc = 10;
        int dexMod = character.GetAttribute(AttributeName.Dexterity).Modifier;
        var armor = character.GetWornArmor();
        var shield = character.GetEquippedShield();

        int armorBonus = armor?.AcBonus ?? 0;
        var equippedWeapon = character.GetEquippedWeapon();
        bool twoHandedEquipped = equippedWeapon?.Tags.HasFlag(WeaponTag.TwoHanded) ?? false;
        int shieldBonus = (shield != null && armor != null && !twoHandedEquipped) ? 1 : 0;
        int focusBonus = FocusEffectAggregator.SumEffects(
            character.Foci, FocusEffectType.AcBonus, character);
        int abilityBonus = ClassAbilityEffectAggregator.SumEffects(
            character.ClassAbilities, FocusEffectType.AcBonus, character);

        return baseAc + armorBonus + dexMod + shieldBonus + focusBonus + abilityBonus;
    }

    public static SkillName GetCombatSkillForWeapon(Weapon weapon)
    {
        return weapon.Tags.HasFlag(WeaponTag.Ranged) 
            ? SkillName.Shoot 
            : SkillName.Stab;
    }

    private static FocusEffectCondition MapSkillToCondition(SkillName skill) => skill switch
    {
        SkillName.Stab => FocusEffectCondition.StabWeapon,
        SkillName.Shoot => FocusEffectCondition.ShootWeapon,
        SkillName.Punch => FocusEffectCondition.PunchWeapon,
        _ => FocusEffectCondition.Always
    };
}
