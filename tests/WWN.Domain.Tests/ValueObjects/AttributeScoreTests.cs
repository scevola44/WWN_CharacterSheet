using FluentAssertions;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.ValueObjects;

public class AttributeScoreTests
{
    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(18)]
    public void ValidScore_Creates(int value)
    {
        var score = new AttributeScore(value);
        score.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Below3_Throws(int value)
    {
        var act = () => new AttributeScore(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(19)]
    [InlineData(100)]
    public void Above18_Throws(int value)
    {
        var act = () => new AttributeScore(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(3, -2)]
    [InlineData(10, 0)]
    [InlineData(14, 1)]
    [InlineData(18, 2)]
    public void ModifierComputed(int value, int expectedMod)
    {
        new AttributeScore(value).Modifier.Should().Be(expectedMod);
    }
}
