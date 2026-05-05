using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class EffortPoolCalculatorTests
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

    private static Character NewCharacter(
        CharacterClass cls = CharacterClass.Mage,
        int intScore = 10,
        int magicRank = -1,
        PartialClass? partialA = null,
        PartialClass? partialB = null)
    {
        var scores = new Dictionary<AttributeName, int>(DefaultScores)
        {
            [AttributeName.Intelligence] = intScore
        };
        var character = Character.Create("Test", cls, scores, "user-1",
            partialA: partialA, partialB: partialB);
        if (magicRank > -1)
            character.SetSkillRank(SkillName.Magic, magicRank);
        return character;
    }

    [Fact]
    public void NonMage_HasZeroEffort()
    {
        var character = NewCharacter(CharacterClass.Warrior);
        EffortPoolCalculator.CalculateMax(character).Should().Be(0);
        EffortPoolCalculator.HasArts(character).Should().BeFalse();
    }

    [Fact]
    public void Mage_BaselineWith10Int_HasOneEffort()
    {
        // INT 10 = +0 modifier, magic rank untrained = 0 → 1 + 0 + 0 = 1
        var character = NewCharacter(CharacterClass.Mage);
        EffortPoolCalculator.CalculateMax(character).Should().Be(1);
    }

    [Fact]
    public void Mage_HighIntAndMagicRank_AddsToEffort()
    {
        // INT 18 = +2, magic rank 3 → 1 + 2 + 3 = 6
        var character = NewCharacter(CharacterClass.Mage, intScore: 18, magicRank: 3);
        EffortPoolCalculator.CalculateMax(character).Should().Be(6);
    }

    [Fact]
    public void Mage_NegativeIntModifier_ClampsAtOne()
    {
        // INT 3 = -2, magic rank 0 → 1 + (-2) + 0 = -1, clamped to 1
        var character = NewCharacter(CharacterClass.Mage, intScore: 3);
        EffortPoolCalculator.CalculateMax(character).Should().Be(1);
    }

    [Fact]
    public void Adventurer_WithPartialMage_GetsEffort()
    {
        var character = NewCharacter(CharacterClass.Adventurer,
            partialA: PartialClass.PartialMage,
            partialB: PartialClass.PartialWarrior);
        EffortPoolCalculator.HasArts(character).Should().BeTrue();
        EffortPoolCalculator.CalculateMax(character).Should().Be(1);
    }

    [Fact]
    public void Adventurer_WithoutPartialMage_HasZeroEffort()
    {
        var character = NewCharacter(CharacterClass.Adventurer,
            partialA: PartialClass.PartialWarrior,
            partialB: PartialClass.PartialExpert);
        EffortPoolCalculator.HasArts(character).Should().BeFalse();
        EffortPoolCalculator.CalculateMax(character).Should().Be(0);
    }
}
