using FluentAssertions;
using WWN.Application.Services;
using WWN.Domain.Enums;

namespace WWN.Application.Tests.Services;

public class LookupsServiceTests
{
    [Fact]
    public void GetAll_ReturnsAllEffortCommitments()
    {
        var svc = new LookupsService();

        var dto = svc.GetAll();

        dto.EffortCommitment.Should().HaveCount(Enum.GetValues<EffortCommitment>().Length);
        dto.EffortCommitment.Select(e => e.Id).Should().BeEquivalentTo(
            Enum.GetValues<EffortCommitment>().Select(v => (int)v));
    }

    [Fact]
    public void GetAll_ReturnsCachedReference()
    {
        var svc = new LookupsService();

        var first = svc.GetAll();
        var second = svc.GetAll();

        ReferenceEquals(first, second).Should().BeTrue(
            "the payload is process-static; callers should share the same instance");
    }

    [Fact]
    public void ETag_IsStableAcrossCalls()
    {
        var svc = new LookupsService();

        svc.ETag.Should().Be(svc.ETag);
        svc.ETag.Should().StartWith("\"").And.EndWith("\"");
    }

    [Fact]
    public void EffortCommitment_NoneEntry_HasIdZero()
    {
        var svc = new LookupsService();

        var none = svc.GetAll().EffortCommitment.Single(v => v.Code == nameof(EffortCommitment.None));
        none.Id.Should().Be(0);
        none.DisplayName.Should().NotBeNullOrEmpty();
    }
}
