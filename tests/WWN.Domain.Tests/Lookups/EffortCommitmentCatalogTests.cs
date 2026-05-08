using FluentAssertions;
using WWN.Domain.Enums;
using WWN.Domain.Lookups;

namespace WWN.Domain.Tests.Lookups;

public class EffortCommitmentCatalogTests
{
    [Fact]
    public void All_ContainsEveryEnumValue()
    {
        var enumValues = Enum.GetValues<EffortCommitment>();
        EffortCommitmentCatalog.All.Select(v => v.Id).Should().BeEquivalentTo(
            enumValues.Select(v => (int)v));
    }

    [Fact]
    public void All_HasUniqueIdsAndCodes()
    {
        var ids = EffortCommitmentCatalog.All.Select(v => v.Id).ToArray();
        ids.Should().OnlyHaveUniqueItems();

        var codes = EffortCommitmentCatalog.All.Select(v => v.Code).ToArray();
        codes.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Codes_MatchEnumNames()
    {
        foreach (var enumValue in Enum.GetValues<EffortCommitment>())
        {
            EffortCommitmentCatalog.Get(enumValue).Code.Should().Be(enumValue.ToString());
        }
    }

    [Fact]
    public void All_IsOrderedBySortOrder()
    {
        EffortCommitmentCatalog.All.Should().BeInAscendingOrder(v => v.SortOrder);
    }

    [Fact]
    public void None_HasIdZero()
    {
        EffortCommitmentCatalog.Get(EffortCommitment.None).Id.Should().Be(0);
    }

    [Fact]
    public void Get_UnknownValue_Throws()
    {
        var act = () => EffortCommitmentCatalog.Get((EffortCommitment)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
