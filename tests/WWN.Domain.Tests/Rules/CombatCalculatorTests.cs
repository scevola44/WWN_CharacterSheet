using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Rules;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.Rules;

public class CombatCalculatorTests
{
    [Fact]
    public void Warrior_Level5_BAB5()
    {
        CombatCalculator.GetBaseAttackBonus(CharacterClass.Warrior, null, null, 5).Should().Be(5);
    }

    [Fact]
    public void Expert_Level5_BAB2()
    {
        CombatCalculator.GetBaseAttackBonus(CharacterClass.Expert, null, null, 5).Should().Be(2);
    }

    [Fact]
    public void Mage_Level4_BAB2()
    {
        CombatCalculator.GetBaseAttackBonus(CharacterClass.Mage, null, null, 4).Should().Be(2);
    }

    [Fact]
    public void PartialWarrior_Level4_BAB3()
    {
        CombatCalculator.GetBaseAttackBonus(CharacterClass.Adventurer,
            PartialClass.PartialWarrior, PartialClass.PartialExpert, 4).Should().Be(3);
    }

    [Fact]
    public void AC_NoArmor_Dex14_Returns11()
    {
        var character = CreateCharacter(dex: 14);
        CombatCalculator.GetArmorClass(character).Should().Be(11);
    }

    [Fact]
    public void AC_Chain_Dex10_Returns14()
    {
        var character = CreateCharacter();
        var armor = new Armor("Chain Mail", 2, 4, false, ItemSlotType.Equipped);
        character.AddItem(armor);
        CombatCalculator.GetArmorClass(character).Should().Be(14);
    }

    [Fact]
    public void AC_ChainAndShield_Dex10_Returns15()
    {
        var character = CreateCharacter();
        var armor = new Armor("Chain Mail", 2, 4, false, ItemSlotType.Equipped);
        var shield = new Armor("Shield", 1, 0, true, ItemSlotType.Equipped);
        character.AddItem(armor);
        character.AddItem(shield);
        CombatCalculator.GetArmorClass(character).Should().Be(15);
    }

    [Fact]
    public void AC_ShieldOnly_NoArmorBonus()
    {
        var character = CreateCharacter();
        var shield = new Armor("Shield", 1, 0, true, ItemSlotType.Equipped);
        character.AddItem(shield);
        // Shield without armor gives no bonus
        CombatCalculator.GetArmorClass(character).Should().Be(10);
    }

    [Fact]
    public void AttackBonus_WarriorL3_Stab2_Str16()
    {
        var character = CreateCharacter(str: 16);
        character.SetLevel(3);
        character.SetSkillRank(SkillName.Stab, 2);

        var sword = new Weapon("Longsword", 1, new DamageDie(1, 8),
            AttributeName.Strength, SkillName.Stab, WeaponTag.Melee);
        character.AddItem(sword);

        // BAB 3 + Stab 2 + STR mod 1 = 6
        CombatCalculator.GetTotalAttackBonus(character, sword).Should().Be(6);
    }

    [Fact]
    public void RangedWeapon_UsesShootAndDex()
    {
        var character = CreateCharacter(dex: 14);
        character.SetLevel(1);
        character.SetSkillRank(SkillName.Shoot, 1);

        var bow = new Weapon("Bow", 1, new DamageDie(1, 6),
            AttributeName.Dexterity, SkillName.Shoot, WeaponTag.Ranged);

        CombatCalculator.GetCombatSkillForWeapon(bow).Should().Be(SkillName.Shoot);
        // BAB 1 + Shoot 1 + DEX mod 1 = 3
        CombatCalculator.GetTotalAttackBonus(character, bow).Should().Be(3);
    }

    [Fact]
    public void AC_WithFocusBonus()
    {
        var character = CreateCharacter();
        var focus = new Focus("Ironhide", 1, new[]
        {
            new FocusEffect(FocusEffectType.AcBonus, 1)
        });
        character.AddFocus(focus);
        CombatCalculator.GetArmorClass(character).Should().Be(11);
    }

    private static Character CreateCharacter(
        int str = 10, int dex = 10, int con = 10,
        int intel = 10, int wis = 10, int cha = 10)
    {
        return Character.Create("Test", CharacterClass.Warrior, new Dictionary<AttributeName, int>
        {
            [AttributeName.Strength] = str,
            [AttributeName.Dexterity] = dex,
            [AttributeName.Constitution] = con,
            [AttributeName.Intelligence] = intel,
            [AttributeName.Wisdom] = wis,
            [AttributeName.Charisma] = cha
        });
    }
}
