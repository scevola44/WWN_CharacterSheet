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

    private static Character NewWarrior() =>
        Character.Create("Warrior", CharacterClass.Warrior, DefaultScores, "user-1");

    private static Character NewPartialMageAdventurer() =>
        Character.Create("Adv", CharacterClass.Adventurer, DefaultScores, "user-1",
            partialA: PartialClass.PartialWarrior, partialB: PartialClass.PartialMage);

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
    public void RestForDay_DoesNotChangeStrain()
    {
        var character = NewMage();
        character.SetStrain(3);

        character.RestForDay();

        character.CurrentStrain.Should().Be(3);
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

    [Fact]
    public void CommitEffort_NonCaster_Throws()
    {
        var character = NewWarrior();
        var act = () => character.CommitEffort(EffortCommitment.Scene, maxEffort: 3);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CommitEffort_AdventurerWithPartialMage_Succeeds()
    {
        var character = NewPartialMageAdventurer();
        character.CommitEffort(EffortCommitment.Scene, maxEffort: 3);
        character.EffortCommittedScene.Should().Be(1);
    }

    [Fact]
    public void CommitEffort_None_Throws()
    {
        var character = NewMage();
        var act = () => character.CommitEffort(EffortCommitment.None, maxEffort: 3);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UseSpellSlot_NonCaster_Throws()
    {
        var character = NewWarrior();
        var act = () => character.UseSpellSlot(1);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UseSpellSlot_BeyondMax_Throws()
    {
        // Level-1 Mage with INT 10 (mod 0) has exactly 1 slot at level 1.
        var character = NewMage();
        character.UseSpellSlot(1);
        var act = () => character.UseSpellSlot(1);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UseSpellSlot_PartialMageAdventurerAtLevel1_HasNoSlots_Throws()
    {
        // Partial Mage Adventurer at level 1 has 0 slots — first call must fail.
        var character = NewPartialMageAdventurer();
        var act = () => character.UseSpellSlot(1);
        act.Should().Throw<InvalidOperationException>();
    }
}
