using FluentAssertions;
using WWN.Domain.Rules;

namespace WWN.Domain.Tests.Rules;

public class AttributeModifierTableTests
{
    [Theory]
    [InlineData(3, -2)]
    [InlineData(4, -1)]
    [InlineData(5, -1)]
    [InlineData(6, -1)]
    [InlineData(7, -1)]
    [InlineData(8, 0)]
    [InlineData(9, 0)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 0)]
    [InlineData(13, 0)]
    [InlineData(14, 1)]
    [InlineData(15, 1)]
    [InlineData(16, 1)]
    [InlineData(17, 1)]
    [InlineData(18, 2)]
    public void GetModifier_ValidScore_ReturnsExpected(int score, int expected)
    {
        AttributeModifierTable.GetModifier(score).Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(19)]
    [InlineData(-1)]
    public void GetModifier_OutOfRange_Throws(int score)
    {
        var act = () => AttributeModifierTable.GetModifier(score);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
