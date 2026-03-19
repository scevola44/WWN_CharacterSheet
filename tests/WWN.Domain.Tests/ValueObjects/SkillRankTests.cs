using FluentAssertions;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Tests.ValueObjects;

public class SkillRankTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void ValidRank_Creates(int level)
    {
        new SkillRank(level).Level.Should().Be(level);
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-10)]
    public void BelowMinus1_Throws(int level)
    {
        var act = () => new SkillRank(level);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    public void Above4_Throws(int level)
    {
        var act = () => new SkillRank(level);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
