using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.Aggregates;

public class CharacterTests
{
    private static readonly Dictionary<AttributeName, int> DefaultScores = new()
    {
        [AttributeName.Strength] = 10,
        [AttributeName.Dexterity] = 10,
        [AttributeName.Constitution] = 10,
        [AttributeName.Intelligence] = 10,
        [AttributeName.Wisdom] = 10,
        [AttributeName.Charisma] = 10
    };

    [Fact]
    public void Create_Initializes6Attributes()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.Attributes.Should().HaveCount(6);
        character.Attributes.Select(a => a.Name).Should()
            .BeEquivalentTo(Enum.GetValues<AttributeName>());
    }

    [Fact]
    public void Create_Initializes16Skills_AtMinus1()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.Skills.Should().HaveCount(21);
        character.Skills.Should().AllSatisfy(s => s.Rank.Level.Should().Be(-1));
    }

    [Fact]
    public void Create_SetsNameAndClass()
    {
        var character = Character.Create("Hero", CharacterClass.Expert, DefaultScores, "user-1");
        character.Name.Should().Be("Hero");
        character.Class.Should().Be(CharacterClass.Expert);
        character.Level.Should().Be(1);
    }

    [Fact]
    public void Create_EmptyName_Throws()
    {
        var act = () => Character.Create("", CharacterClass.Warrior, DefaultScores, "user-1");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Adventurer_RequiresPartials()
    {
        var act = () => Character.Create("Test", CharacterClass.Adventurer, DefaultScores, "user-1");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Adventurer_WithPartials_Succeeds()
    {
        var character = Character.Create("Test", CharacterClass.Adventurer, DefaultScores, "user-1",
            partialA: PartialClass.PartialWarrior, partialB: PartialClass.PartialExpert);
        character.PartialClassA.Should().Be(PartialClass.PartialWarrior);
        character.PartialClassB.Should().Be(PartialClass.PartialExpert);
    }

    [Fact]
    public void Create_NonAdventurer_WithPartials_Throws()
    {
        var act = () => Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1",
            partialA: PartialClass.PartialWarrior);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(PartialClass.PartialWarrior)]
    [InlineData(PartialClass.PartialExpert)]
    [InlineData(PartialClass.PartialMage)]
    public void Create_AdventurerWithDuplicatePartials_Throws(PartialClass partial)
    {
        var act = () => Character.Create("Test", CharacterClass.Adventurer, DefaultScores, "user-1",
            partialA: partial, partialB: partial);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*distinct*");
    }

    [Fact]
    public void SetAttribute_Valid_UpdatesScore()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetAttribute(AttributeName.Strength, 16);
        character.GetAttribute(AttributeName.Strength).Score.Value.Should().Be(16);
        character.GetAttribute(AttributeName.Strength).Modifier.Should().Be(1);
    }

    [Fact]
    public void SetSkillRank_Valid_UpdatesRank()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetSkillRank(SkillName.Stab, 2);
        character.GetSkill(SkillName.Stab).Rank.Level.Should().Be(2);
    }

    [Fact]
    public void AddCustomSkill_Adds()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.AddCustomSkill("Alchemy", 0);
        character.Skills.Should().HaveCount(22);
        character.Skills.Last().CustomName.Should().Be("Alchemy");
    }

    [Fact]
    public void TakeDamage_ReducesCurrentHp()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetHitPoints(10, 10);
        character.TakeDamage(3);
        character.CurrentHitPoints.Should().Be(7);
    }

    [Fact]
    public void TakeDamage_DoesNotGoBelowZero()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetHitPoints(10, 5);
        character.TakeDamage(100);
        character.CurrentHitPoints.Should().Be(0);
    }

    [Fact]
    public void Heal_DoesNotExceedMax()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetHitPoints(10, 5);
        character.Heal(100);
        character.CurrentHitPoints.Should().Be(10);
    }

    [Fact]
    public void SetHitPoints_InvalidMax_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var act = () => character.SetHitPoints(0, 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetHitPoints_CurrentExceedsMax_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var act = () => character.SetHitPoints(5, 10);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EquipItem_SetsSlotToEquipped()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var item = new Item("Rope", 1);
        character.AddItem(item);
        character.EquipItem(item.Id);
        character.Inventory.First().SlotType.Should().Be(ItemSlotType.Equipped);
    }

    [Fact]
    public void UnequipItem_SetsSlotToStowed()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var item = new Item("Rope", 1, ItemSlotType.Equipped);
        character.AddItem(item);
        character.UnequipItem(item.Id);
        character.Inventory.First().SlotType.Should().Be(ItemSlotType.Stowed);
    }

    [Fact]
    public void RemoveItem_RemovesFromInventory()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var item = new Item("Rope", 1);
        character.AddItem(item);
        character.RemoveItem(item.Id);
        character.Inventory.Should().BeEmpty();
    }

    [Fact]
    public void AddFocus_WithEffects_AppendsToList()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var focus = new Focus("Alert", 1, new[]
        {
            new FocusEffect(FocusEffectType.Initiative, 1)
        });
        character.AddFocus(focus);
        character.Foci.Should().HaveCount(1);
        character.Foci[0].Name.Should().Be("Alert");
    }

    [Fact]
    public void RemoveFocus_Removes()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var focus = new Focus("Alert", 1, Array.Empty<FocusEffect>());
        character.AddFocus(focus);
        character.RemoveFocus(focus.Id);
        character.Foci.Should().BeEmpty();
    }

    [Fact]
    public void SetLevel_Valid_Updates()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetLevel(5);
        character.Level.Should().Be(5);
    }

    [Fact]
    public void SetLevel_OutOfRange_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var act = () => character.SetLevel(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WithLevel_SetsLevel()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores,
            userId: "user-1", level: 5);
        character.Level.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Create_LevelOutOfRange_Throws(int level)
    {
        var act = () => Character.Create("Test", CharacterClass.Warrior, DefaultScores,
            userId: "user-1", level: level);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void LevelUp_IncrementsLevelAndAddsHp()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores,
            userId: "user-1", maxHitPoints: 6);
        character.LevelUp(5);
        character.Level.Should().Be(2);
        character.MaxHitPoints.Should().Be(11);
        character.CurrentHitPoints.Should().Be(11);
    }

    [Fact]
    public void LevelUp_AtMaxLevel_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores,
            userId: "user-1", level: 10);
        var act = () => character.LevelUp(4);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void LevelUp_HpGainBelow1_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores,
            userId: "user-1");
        var act = () => character.LevelUp(0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetEquippedWeapon_ReturnsEquippedWeapon()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var sword = new Weapon("Sword", 1, new DamageDie(1, 8),
            AttributeName.Strength, SkillName.Stab, WeaponTag.Melee, slotType: ItemSlotType.Equipped);
        character.AddItem(sword);
        character.GetEquippedWeapon().Should().NotBeNull();
        character.GetEquippedWeapon()!.Name.Should().Be("Sword");
    }

    [Fact]
    public void GetWornArmor_ReturnsEquippedNonShieldArmor()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var armor = new Armor("Plate", 3, 6, false, ItemSlotType.Equipped);
        character.AddItem(armor);
        character.GetWornArmor().Should().NotBeNull();
        character.GetWornArmor()!.AcBonus.Should().Be(6);
    }

    [Fact]
    public void GetEquippedShield_ReturnsOnlyShields()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var shield = new Armor("Shield", 1, 0, true, ItemSlotType.Equipped);
        var armor = new Armor("Plate", 3, 6, false, ItemSlotType.Equipped);
        character.AddItem(armor);
        character.AddItem(shield);
        character.GetEquippedShield().Should().NotBeNull();
        character.GetEquippedShield()!.IsShield.Should().BeTrue();
    }

    [Fact]
    public void SetStrain_Valid_UpdatesCurrentStrain()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetStrain(3);
        character.CurrentStrain.Should().Be(3);
    }

    [Fact]
    public void SetStrain_Zero_IsValid()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetStrain(5);
        character.SetStrain(0);
        character.CurrentStrain.Should().Be(0);
    }

    [Fact]
    public void SetStrain_EqualToConScore_IsValid()
    {
        // DefaultScores has CON = 10
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.SetStrain(10);
        character.CurrentStrain.Should().Be(10);
    }

    [Fact]
    public void SetStrain_ExceedsConScore_Throws()
    {
        // DefaultScores has CON = 10
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var act = () => character.SetStrain(11);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetStrain_Negative_Throws()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        var act = () => character.SetStrain(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetStrain_DefaultIsZero()
    {
        var character = Character.Create("Test", CharacterClass.Warrior, DefaultScores, "user-1");
        character.CurrentStrain.Should().Be(0);
    }
}
