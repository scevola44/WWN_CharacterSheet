using FluentAssertions;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Rules;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.Rules;

public class FocusEffectAggregatorTests
{
    [Fact]
    public void NoFoci_ReturnsZero()
    {
        FocusEffectAggregator.SumEffects(Array.Empty<Focus>(), FocusEffectType.AttackBonus)
            .Should().Be(0);
    }

    [Fact]
    public void SingleFocus_AttackBonus_Sums()
    {
        var foci = new[]
        {
            new Focus("Weapon Focus", 1, new[] { new FocusEffect(FocusEffectType.AttackBonus, 1) })
        };
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus).Should().Be(1);
    }

    [Fact]
    public void MultipleFoci_DifferentTypes_FiltersCorrectly()
    {
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

        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AttackBonus).Should().Be(2);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.AcBonus).Should().Be(1);
        FocusEffectAggregator.SumEffects(foci, FocusEffectType.DamageBonus).Should().Be(2);
    }

    [Fact]
    public void SkillBonus_TargetsCorrectSkill()
    {
        var foci = new[]
        {
            new Focus("Specialist", 1, new[]
            {
                new FocusEffect(FocusEffectType.SkillBonus, 1, TargetSkill: SkillName.Sneak),
                new FocusEffect(FocusEffectType.SkillBonus, 2, TargetSkill: SkillName.Notice)
            })
        };

        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Sneak).Should().Be(1);
        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Notice).Should().Be(2);
        FocusEffectAggregator.SumSkillEffects(foci, SkillName.Stab).Should().Be(0);
    }

    [Fact]
    public void AttributeBonus_TargetsCorrectAttribute()
    {
        var foci = new[]
        {
            new Focus("Mighty", 1, new[]
            {
                new FocusEffect(FocusEffectType.AttributeBonus, 1, TargetAttribute: AttributeName.Strength)
            })
        };

        FocusEffectAggregator.SumAttributeEffects(foci, AttributeName.Strength).Should().Be(1);
        FocusEffectAggregator.SumAttributeEffects(foci, AttributeName.Dexterity).Should().Be(0);
    }
}
