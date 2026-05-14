using Microsoft.Extensions.Caching.Memory;
using WWN.Application.DTOs;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.Rules;

namespace WWN.Application.Services;

public class CharacterDetailMapper(
    IFocusDefinitionRepository focusDefinitionRepository,
    IClassAbilityRepository classAbilityRepository,
    IMemoryCache cache)
{
    public const string FocusDefsKey = "focus-definitions";
    public const string ClassAbilitiesKey = "class-abilities";

    private static readonly MemoryCacheEntryOptions s_defCacheOptions =
        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

    public async Task<CharacterDetailDto> SyncAndMapAsync(Character character, CancellationToken cancellationToken = default)
    {
        var definitions = await cache.GetOrCreateAsync(
            FocusDefsKey,
            entry => { entry.SetOptions(s_defCacheOptions); return focusDefinitionRepository.GetAllAsync(cancellationToken); })
            ?? [];
        var defsByName = definitions.ToDictionary(d => d.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var focus in character.Foci)
        {
            if (defsByName.TryGetValue(focus.Name, out var def))
            {
                var effects = focus.Level >= 2
                    ? def.Level1Effects.Concat(def.Level2Effects)
                    : def.Level1Effects;
                focus.SetEffects(effects);
            }
        }
        return await MapToDetailDtoAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> MapToDetailDtoAsync(
        Character character,
        CancellationToken cancellationToken = default)
    {
        var allAbilities = await cache.GetOrCreateAsync(
            ClassAbilitiesKey,
            entry => { entry.SetOptions(s_defCacheOptions); return classAbilityRepository.GetAllAsync(cancellationToken); })
            ?? [];
        var ownerKeys = GetAbilityOwnerKeys(character);

        var activeAbilities = allAbilities
            .Where(a => ownerKeys.Contains(a.ClassOwner) && a.MinLevel <= character.Level)
            .ToList();

        character.LoadClassAbilityDefinitions(activeAbilities);

        var classAbilities = activeAbilities
            .OrderBy(a => a.MinLevel)
            .ThenBy(a => a.Name)
            .Select(a => new ClassAbilityDto
            {
                Name = a.Name,
                Description = a.Description,
                MinLevel = a.MinLevel,
                ClassOwner = a.ClassOwner
            })
            .ToList();

        var calculatedStats = CharacterSheetCalculator.Calculate(character);
        var strScore = character.GetAttribute(AttributeName.Strength).Score.Value;
        return new CharacterDetailDto
        {
            Id = character.Id,
            Name = character.Name,
            Background = character.Background,
            Origin = character.Origin,
            Class = character.Class.ToString(),
            PartialClassA = character.PartialClassA?.ToString(),
            PartialClassB = character.PartialClassB?.ToString(),
            Level = character.Level,
            MaxHitPoints = character.MaxHitPoints,
            CurrentHitPoints = character.CurrentHitPoints,
            CurrentStrain = character.CurrentStrain,
            ExperiencePoints = character.ExperiencePoints,
            Notes = character.Notes,
            Attributes = character.Attributes.Select(attribute => new AttributeDto
            {
                Name = attribute.Name.ToString(),
                Score = attribute.Score.Value,
                Modifier = attribute.Modifier
            }).ToList(),
            Skills = character.Skills.Select(skill => new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name.ToString(),
                CustomName = skill.CustomName,
                Level = skill.Rank.Level
                    + FocusEffectAggregator.SumSkillEffects(character.Foci, skill.Name, character)
                    + ClassAbilityEffectAggregator.SumSkillEffects(character.ClassAbilities, skill.Name, character)
            }).ToList(),
            Foci = character.Foci.Select(focus => new FocusDto
            {
                Id = focus.Id,
                Name = focus.Name,
                Level = focus.Level,
                Effects = focus.Effects.Select(focusEffect => new FocusEffectDto
                {
                    Type = focusEffect.Type.ToString(),
                    NumericValue = focusEffect.NumericValue,
                    TargetSkill = focusEffect.TargetSkill?.ToString(),
                    TargetAttribute = focusEffect.TargetAttribute?.ToString(),
                    Description = focusEffect.Description
                }).ToList()
            }).ToList(),
            Inventory = character.Inventory.Select(MapItemDto).ToList(),
            Spellbook = character.Spellbook.Select(k => new KnownSpellDto
            {
                Id = k.Id,
                SpellId = k.SpellId,
                Spell = SpellService.MapToDto(k.Spell)
            }).ToList(),
            SpellSlots = GetSpellSlotsInfo(character),
            Arts = character.KnownArts.Select(k => new KnownArtDto
            {
                Id = k.Id,
                ArtId = k.ArtId,
                Art = ArtService.MapToDto(k.Art)
            }).ToList(),
            Effort = GetEffortInfo(character),
            ClassAbilities = classAbilities,
            DerivedStats = calculatedStats,
            EncumbranceSummary = new EncumbranceSummaryDto
            {
                ReadiedLoad = EncumbranceCalculator.GetReadiedLoad(character.Inventory),
                MaxReadied = EncumbranceCalculator.GetMaxReadied(strScore),
                StowedLoad = EncumbranceCalculator.GetStowedLoad(character.Inventory),
                MaxStowed = EncumbranceCalculator.GetMaxStowed(strScore)
            }
        };
    }

    private static EffortInfoDto? GetEffortInfo(Character character)
    {
        if (!EffortPoolCalculator.HasArts(character)) return null;
        return new EffortInfoDto
        {
            Max = EffortPoolCalculator.CalculateMax(character),
            Scene = character.EffortCommittedScene,
            Day = character.EffortCommittedDay,
            Sustained = character.EffortCommittedSustained
        };
    }

    private static HashSet<string> GetAbilityOwnerKeys(Character character)
    {
        if (character.Class != CharacterClass.Adventurer)
            return [character.Class.ToString()];

        var keys = new HashSet<string>();
        if (character.PartialClassA.HasValue) keys.Add(character.PartialClassA.Value.ToString());
        if (character.PartialClassB.HasValue) keys.Add(character.PartialClassB.Value.ToString());
        return keys;
    }

    private static SpellSlotInfoDto? GetSpellSlotsInfo(Character character)
    {
        if (character.Class != CharacterClass.Mage && !HasPartialMage(character))
            return null;

        var intModifier = character.GetAttribute(AttributeName.Intelligence).Modifier;
        var effectiveClass = character.Class == CharacterClass.Mage ? CharacterClass.Mage : CharacterClass.Adventurer;
        var available = SpellSlotCalculator.CalculateSlots(effectiveClass, character.Level, intModifier);

        return new SpellSlotInfoDto
        {
            Available = available,
            Used = character.SpellSlotsUsed
        };
    }

    private static bool HasPartialMage(Character character)
    {
        return character.PartialClassA == PartialClass.PartialMage
               || character.PartialClassB == PartialClass.PartialMage;
    }

    private static ItemDto MapItemDto(Item item)
    {
        var dto = new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Encumbrance = item.Encumbrance,
            SlotType = item.SlotType.ToString(),
            Quantity = item.Quantity,
        };

        if (item is Weapon weapon)
        {
            dto = dto with
            {
                ItemType = "Weapon",
                DamageDie = weapon.DamageDie.ToString(),
                AttributeModifier = weapon.AttributeModifier.ToString(),
                CombatSkill = weapon.CombatSkill.ToString(),
                ShockDamage = weapon.Shock?.Damage,
                ShockAcThreshold = weapon.Shock?.AcThreshold,
                WeaponType = weapon.WeaponType.ToString(),
                Tags = weapon.Tags.ToString()
            };
        }
        else if (item is Armor armor)
        {
            dto = dto with
            {
                ItemType = "Armor",
                AcBonus = armor.AcBonus,
                IsShield = armor.IsShield
            };
        }

        return dto;
    }
}
