using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Rules;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.Rules;

public class FocusEffectAggregatorTests
{
    private static Character MinimalCharacter()
    {
        var scores = Enum.GetValues<AttributeName>().ToDictionary(a => a, _ => 10);
        return Character.Create("Test", CharacterClass.Expert, scores);
    }

    [Fact]
    public void NoFoci_ReturnsZero()
    {
        FocusEffectAggregator.SumEffects(Array.Empty<Focus>(), FocusEffectType.AttackBonus, MinimalCharacter())
            .Should().Be(0);
    }

    [Fact]
    public void SingleFocus_AttackBonus_Sums()
    {
        var foci = new[]
        {
            new Focus("Weapon Focus", 1, new[] { new FocusEffect(FocusEffectType.AttackBonus, 1) })
        };
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus, MinimalCharacter()).Should().Be(1);
    }

    [Fact]
    public void MultipleFoci_DifferentTypes_FiltersCorrectly()
    {
        var character = MinimalCharacter();
        var foci = new[]
        {
            new Focus("Weapon Focus", 1, new[] { new FocusEffect(FocusEffectType.AttackBonus, 1) }),
            new Focus("Ironhide", 1, new[] { new FocusEffect(FocusEffectType.AcBonus, 1) }),
            new Focus("Weapon Mastery", 2, new[]
            {
                new FocusEffect(FocusEffectType.AttackBonus, 1),
                new FocusEffect(FocusEffectType.DamageBonus, 2)
            })
        };

        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus, character).Should().Be(2);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AcBonus, character).Should().Be(1);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.DamageBonus, character).Should().Be(2);
    }

    [Fact]
    public void SkillBonus_TargetsCorrectSkill()
    {
        var character = MinimalCharacter();
        var foci = new[]
        {
            new Focus("Specialist", 1, new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Sneak),
                new FocusEffect(FocusEffectType.SkillBonus, 2, TargetSkill: SkillName.Notice)
            })
        };

        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Sneak, character).Should().Be(1);
        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Notice, character).Should().Be(2);
        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Stab, character).Should().Be(0);
    }

    [Fact]
    public void AttributeBonus_TargetsCorrectAttribute()
    {
        var character = MinimalCharacter();
        var foci = new[]
        {
            new Focus("Mighty", 1, new[]
            {
                new FocusEffect(FocusEffectType.AttributeBonus, 1, TargetAttribute: AttributeName.Strength)
            })
        };

        FocusEffectAggregator.SumAttributeEffects(foci, AttributeName.Strength, character).Should().Be(1);
        FocusEffectAggregator.SumAttributeEffects(foci, AttributeName.Dexterity, character).Should().Be(0);
    }

    [Fact]
    public void Condition_StabWeapon_OnlyAppliesForStab()
    {
        var character = MinimalCharacter();
        var foci = new[]
        {
            new Focus("Armsmaster", 1, new[]
            {
                new FocusEffect(FocusEffectType.AttackBonus, 1, Condition: FocusEffectCondition.StabWeapon)
            })
        };

        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus, character, FocusEffectCondition.StabWeapon)
            .Should().Be(1);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus, character, FocusEffectCondition.ShootWeapon)
            .Should().Be(0);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus, character)
            .Should().Be(0); // Always context doesn't match StabWeapon
    }

    [Fact]
    public void Condition_Conditional_RequiresConditionalActive()
    {
        var character = MinimalCharacter();
        var inactiveFocus = new Focus("Passive", 1, new[]
        {
            new FocusEffect(FocusEffectType.AttackBonus, 2, Condition: FocusEffectCondition.Conditional)
        });
        // ConditionalActive defaults to false
        FocusEffectAggregator.SumEffects(new[] { inactiveFocus }, FocusEffectType.AttackBonus, character)
            .Should().Be(0);

        inactiveFocus.SetConditionalActive(true);
        FocusEffectAggregator.SumEffects(new[] { inactiveFocus }, FocusEffectType.AttackBonus, character)
            .Should().Be(2);
    }

    [Fact]
    public void ValueType_Level_UsesCharacterLevel()
    {
        var scores = Enum.GetValues<AttributeName>().ToDictionary(a => a, _ => 10);
        var character = Character.Create("Test", CharacterClass.Warrior, scores);
        character.SetLevel(5);

        var foci = new[]
        {
            new Focus("Scaling Focus", 1, new[]
            {
                new FocusEffect(FocusEffectType.DamageBonus, 0, FocusEffectValueType.Level)
            })
        };

        FocusEffectAggregator.SumEffects(foci, FocusEffectType.DamageBonus, character)
            .Should().Be(5);
    }
}
