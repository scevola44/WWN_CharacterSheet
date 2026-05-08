using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;

namespace WWN.Application.Services;

/// <summary>
/// Seeds the Arts table with a starter selection inspired by the
/// Worlds Without Number Free Edition rulebook (Sine Nomine Publishing).
/// </summary>
public class ArtDefinitionSeeder(IArtRepository artRepository)
{
    public async Task SeedIfEmptyAsync(CancellationToken cancellationToken = default)
    {
        if (await artRepository.AnyAsync(cancellationToken)) return;

        foreach (var art in CreateDefaultArts())
            await artRepository.AddAsync(art, cancellationToken);
    }

    private static IEnumerable<Art> CreateDefaultArts()
    {
        yield return new Art(
            name: "Sense Magic",
            description:
                "As an Instant action you perceive nearby magical energy and gain a one-sentence " +
                "impression of any standing magics or magical items you focus on. The effect lasts " +
                "while you concentrate on it.",
            minLevel: 1,
            sourceId: 1,
            summary: "Perceive magic at a glance.");

        yield return new Art(
            name: "Read Magic",
            description:
                "You can read and understand magical scripts and runes. Most workings reveal their " +
                "general purpose, though deeply hidden secrets may resist this Art.",
            minLevel: 1,
            sourceId: 1,
            summary: "Comprehend written magic.");

        yield return new Art(
            name: "Magic Sense",
            description:
                "Commit Effort for the scene to extend Sense Magic across an entire location. " +
                "You can pinpoint the source of any active magic within sight and identify the most " +
                "powerful standing dweomer in the area.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Scene,
            summary: "Pinpoint magical sources for a scene.");

        yield return new Art(
            name: "Counter Magic",
            description:
                "Commit Effort for the day as an Instant action when an enemy mage casts a spell " +
                "you can perceive. Make an opposed Wis/Magic check against the caster's relevant " +
                "Magic check; if you win, the spell fails entirely.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Day,
            summary: "Cancel an enemy spell.");

        yield return new Art(
            name: "Resist Magic",
            description:
                "Commit Effort for as long as you wish to maintain it. While the Effort is " +
                "committed, you gain a bonus equal to half your Magic skill (rounded up) to all " +
                "Mental saving throws against magical effects.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Sustained,
            summary: "Sustained bonus to Mental saves vs. magic.");

        yield return new Art(
            name: "Greater Working",
            description:
                "Commit Effort for the day when you cast a spell. The spell is treated as one " +
                "level higher for the purposes of duration, range, or damage, at the GM's " +
                "discretion.",
            minLevel: 3,
            sourceId: 1,
            effortCost: EffortCommitment.Day,
            summary: "Empower a spell at the cost of Effort.");

        yield return new Art(
            name: "Healing Touch",
            description:
                "Commit Effort for the scene and touch a wounded creature. They regain hit points " +
                "equal to your character level plus your Magic skill. Each target may benefit from " +
                "this Art only once per day.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Scene,
            summary: "Heal a creature on touch.");

        yield return new Art(
            name: "Command the Dead",
            description:
                "Commit Effort for the scene to bend nearby undead to your will. Affected undead " +
                "with hit dice equal to or less than twice your character level make a Mental save " +
                "or obey reasonable commands until the scene ends or the Effort is released.",
            minLevel: 2,
            sourceId: 1,
            effortCost: EffortCommitment.Scene,
            summary: "Compel nearby undead.");

        yield return new Art(
            name: "Elemental Favor",
            description:
                "Make a direct appeal to a non-magical mass of earth, stone, water, flame, or air " +
                "no larger than a ten-foot cube. The element shifts shape, parts, or ignites at " +
                "your direction for one round. No Effort cost for trivial requests; Commit Effort " +
                "for the scene to sustain a larger working.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Scene,
            summary: "Move or shape elements briefly.");

        yield return new Art(
            name: "Push Effort",
            description:
                "Commit Effort for the day as an Instant action when you make a Magic skill check. " +
                "Add your character level to the result of the check.",
            minLevel: 1,
            sourceId: 1,
            effortCost: EffortCommitment.Day,
            summary: "Boost a Magic check by your level.");
    }
}
