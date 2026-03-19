using FluentAssertions;
using WWN.Domain.Enums;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class HitPointCalculatorTests
{
    [Fact]
    public void Warrior_Returns2()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Warrior, null, null).Should().Be(2);
    }

    [Fact]
    public void Expert_Returns0()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Expert, null, null).Should().Be(0);
    }

    [Fact]
    public void Mage_ReturnsMinus1()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Mage, null, null).Should().Be(-1);
    }

    [Fact]
    public void Adventurer_PartialWarrior_Returns2()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Adventurer,
            PartialClass.PartialWarrior, PartialClass.PartialExpert).Should().Be(2);
    }

    [Fact]
    public void Adventurer_PartialMage_ReturnsMinus1()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Adventurer,
            PartialClass.PartialExpert, PartialClass.PartialMage).Should().Be(-1);
    }

    [Fact]
    public void Adventurer_ExpertExpert_Returns0()
    {
        HitPointCalculator.GetHitDieModifier(CharacterClass.Adventurer,
            PartialClass.PartialExpert, PartialClass.PartialExpert).Should().Be(0);
    }
}
