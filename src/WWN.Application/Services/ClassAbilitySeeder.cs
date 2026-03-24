using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

/// <summary>
/// Seeds the ClassAbilityDefinitions table with the standard class abilities from the
/// Worlds Without Number Free Edition rulebook (Sine Nomine Publishing).
/// </summary>
public class ClassAbilitySeeder(IClassAbilityRepository repository)
{
    public async Task SeedIfEmptyAsync(CancellationToken ct = default)
    {
        if (await repository.AnyAsync(ct)) return;

        foreach (var ability in CreateDefaultAbilities())
            await repository.AddAsync(ability, ct);
    }

    private static IEnumerable<ClassAbilityDefinition> CreateDefaultAbilities()
    {
        // Warrior
        yield return new ClassAbilityDefinition(
            name: "Killing Blow",
            description:
                "Add half your character level (rounded up) to all damage rolls, including Shock damage. " +
                "This bonus applies to every attack, spell, or special ability that inflicts damage.",
            minLevel: 1,
            classOwner: "Warrior",
            effects:
            [
                new ClassAbilityEffect(
                    Type: FocusEffectType.DamageBonus,
                    NumericValue: 0,
                    ValueType: FocusEffectValueType.HalfLevelRoundedUp,
                    Description: "Killing Blow damage bonus")
            ]);

        yield return new ClassAbilityDefinition(
            name: "Veteran's Luck",
            description:
                "Once per scene as an Instant action, either convert one of your missed attack rolls into a hit, " +
                "or force an enemy's successful attack roll against you to miss instead.",
            minLevel: 1,
            classOwner: "Warrior");

        // Partial Warrior (Adventurer)
        yield return new ClassAbilityDefinition(
            name: "Veteran's Luck",
            description:
                "Once per scene as an Instant action, either convert one of your missed attack rolls into a hit, " +
                "or force an enemy's successful attack roll against you to miss instead.",
            minLevel: 1,
            classOwner: "PartialWarrior");

        // Expert
        yield return new ClassAbilityDefinition(
            name: "Masterful Expertise",
            description:
                "Once per scene as an Instant action, reroll any failed non-combat skill check and take the " +
                "better of the two results.",
            minLevel: 1,
            classOwner: "Expert");

        yield return new ClassAbilityDefinition(
            name: "Quick Learner",
            description:
                "Gain one additional skill point each time you advance a level. This point may only be spent " +
                "on non-combat skills or on raising an attribute.",
            minLevel: 2,
            classOwner: "Expert");

        // Partial Expert (Adventurer)
        yield return new ClassAbilityDefinition(
            name: "Masterful Expertise",
            description:
                "Once per scene as an Instant action, reroll any failed non-combat skill check and take the " +
                "better of the two results.",
            minLevel: 1,
            classOwner: "PartialExpert");

        // Mage
        yield return new ClassAbilityDefinition(
            name: "Arcane Tradition",
            description:
                "Choose one magical tradition at character creation. You gain full spell progression for that " +
                "tradition, including Arts (lesser magical abilities) and a growing pool of spell slots. " +
                "Magic is gained as a bonus skill.",
            minLevel: 1,
            classOwner: "Mage");

        // Partial Mage (Adventurer)
        yield return new ClassAbilityDefinition(
            name: "Arcane Tradition",
            description:
                "Choose one magical tradition at character creation, including traditions exclusive to partial " +
                "mages. You gain the Adventurer spell progression for that tradition (fewer spell slots than a " +
                "full Mage). Magic is gained as a bonus skill.",
            minLevel: 1,
            classOwner: "PartialMage");
    }
}
