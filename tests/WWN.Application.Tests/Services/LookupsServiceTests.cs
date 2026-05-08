using FluentAssertions;
using NSubstitute;
using WWN.Application.Services;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;

namespace WWN.Application.Tests.Services;

public class LookupsServiceTests
{
    private static IArtSourceRepository CreateMockArtSourceRepository()
    {
        var repo = Substitute.For<IArtSourceRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ArtSource>>(new[]
            {
                new ArtSource("Mage", "Mage", null, 1),
                new ArtSource("PartialMage", "Partial Mage", null, 2)
            }));
        return repo;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEffortCommitments()
    {
        var mockRepo = CreateMockArtSourceRepository();
        var svc = new LookupsService(mockRepo);

        var dto = await svc.GetAllAsync();

        dto.EffortCommitment.Should().HaveCount(Enum.GetValues<EffortCommitment>().Length);
        dto.EffortCommitment.Select(e => e.Id).Should().BeEquivalentTo(
            Enum.GetValues<EffortCommitment>().Select(v => (int)v));
    }

    [Fact]
    public async Task GetAllAsync_IncludesArtSources()
    {
        var mockRepo = CreateMockArtSourceRepository();
        var svc = new LookupsService(mockRepo);

        var dto = await svc.GetAllAsync();

        dto.ArtSources.Should().HaveCount(2);
        dto.ArtSources.Select(s => s.Code).Should().BeEquivalentTo("Mage", "PartialMage");
    }

    [Fact]
    public async Task ComputeETag_IsStableForSamePayload()
    {
        var mockRepo = CreateMockArtSourceRepository();
        var svc = new LookupsService(mockRepo);

        var dto1 = await svc.GetAllAsync();
        var dto2 = await svc.GetAllAsync();

        var etag1 = LookupsService.ComputeETag(dto1);
        var etag2 = LookupsService.ComputeETag(dto2);

        etag1.Should().Be(etag2);
        etag1.Should().StartWith("\"").And.EndWith("\"");
    }

    [Fact]
    public async Task EffortCommitment_NoneEntry_HasIdZero()
    {
        var mockRepo = CreateMockArtSourceRepository();
        var svc = new LookupsService(mockRepo);

        var dto = await svc.GetAllAsync();
        var none = dto.EffortCommitment.Single(v => v.Code == nameof(EffortCommitment.None));
        none.Id.Should().Be(0);
        none.DisplayName.Should().NotBeNullOrEmpty();
    }
}
