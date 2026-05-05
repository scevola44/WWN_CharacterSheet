using FluentAssertions;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;

namespace WWN.Domain.Tests.Aggregates;

public class CharacterArtsTests
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

    private static Character NewMage() =>
        Character.Create("Mage", CharacterClass.Mage, DefaultScores, "user-1");

    [Fact]
    public void LearnArt_AddsToKnownArts()
    {
        var character = NewMage();
        var artId = Guid.NewGuid();
        character.LearnArt(new KnownArt(artId));
        character.KnownArts.Should().HaveCount(1);
        character.KnownArts[0].ArtId.Should().Be(artId);
    }

    [Fact]
    public void LearnArt_DuplicateArtId_Throws()
    {
        var character = NewMage();
        var artId = Guid.NewGuid();
        character.LearnArt(new KnownArt(artId));
        var act = () => character.LearnArt(new KnownArt(artId));
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ForgetArt_Removes()
    {
        var character = NewMage();
        var artId = Guid.NewGuid();
        character.LearnArt(new KnownArt(artId));
        character.ForgetArt(artId);
        character.KnownArts.Should().BeEmpty();
    }

    [Fact]
    public void ForgetArt_Unknown_Throws()
    {
        var character = NewMage();
        var act = () => character.ForgetArt(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CommitEffort_IncrementsTheRightBucket()
    {
        var character = NewMage();
        character.CommitEffort(EffortCommitment.Scene, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Day, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Sustained, maxEffort: 3);
        character.EffortCommittedScene.Should().Be(1);
        character.EffortCommittedDay.Should().Be(1);
        character.EffortCommittedSustained.Should().Be(1);
    }

    [Fact]
    public void CommitEffort_ExceedingMax_Throws()
    {
        var character = NewMage();
        character.CommitEffort(EffortCommitment.Day, maxEffort: 1);
        var act = () => character.CommitEffort(EffortCommitment.Scene, maxEffort: 1);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EndScene_ClearsOnlySceneBucket()
    {
        var character = NewMage();
        character.CommitEffort(EffortCommitment.Scene, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Day, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Sustained, maxEffort: 3);
        character.EndScene();
        character.EffortCommittedScene.Should().Be(0);
        character.EffortCommittedDay.Should().Be(1);
        character.EffortCommittedSustained.Should().Be(1);
    }

    [Fact]
    public void RestForDay_ClearsSceneAndDay_AndRestoresSpellSlots()
    {
        var character = NewMage();
        character.CommitEffort(EffortCommitment.Scene, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Day, maxEffort: 3);
        character.CommitEffort(EffortCommitment.Sustained, maxEffort: 3);
        character.UseSpellSlot(1);

        character.RestForDay();

        character.EffortCommittedScene.Should().Be(0);
        character.EffortCommittedDay.Should().Be(0);
        character.EffortCommittedSustained.Should().Be(1);
        character.SpellSlotsUsed.Should().BeEquivalentTo(new[] { 0, 0, 0, 0, 0, 0 });
    }

    [Fact]
    public void ReleaseSustainedEffort_DecrementsSustainedOnly()
    {
        var character = NewMage();
        character.CommitEffort(EffortCommitment.Sustained, maxEffort: 3, amount: 2);
        character.ReleaseSustainedEffort();
        character.EffortCommittedSustained.Should().Be(1);
    }

    [Fact]
    public void ReleaseSustainedEffort_AboveCommitted_Throws()
    {
        var character = NewMage();
        var act = () => character.ReleaseSustainedEffort();
        act.Should().Throw<InvalidOperationException>();
    }
}
