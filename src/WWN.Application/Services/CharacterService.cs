using WWN.Application.DTOs;
using WWN.Application.Factories;
using WWN.Application.Helpers;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.Rules;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

public class CharacterService(
    ICharacterRepository characterRepository,
    IFocusDefinitionRepository focusDefinitionRepository,
    IClassAbilityRepository classAbilityRepository)
{
    public async Task<CharacterDetailDto?> GetCharacterAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var character = await characterRepository.GetByIdAsync(id, userId, cancellationToken);
        return character is null ? null : await SyncAndMap(character, cancellationToken);
    }

    public async Task<IReadOnlyList<CharacterSummaryDto>> ListCharactersAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var characters = await characterRepository.GetAllSummariesAsync(userId, cancellationToken);
        return characters.Select(c => new CharacterSummaryDto
        {
            Id = c.Id,
            Name = c.Name,
            Class = c.Class.ToString(),
            Level = c.Level,
            CurrentHitPoints = c.CurrentHitPoints,
            MaxHitPoints = c.MaxHitPoints
        }).ToList();
    }

    public async Task<Guid> CreateCharacterAsync(
        CreateCharacterRequest request,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var characterClass = EnumParser.Parse<CharacterClass>(request.Class, nameof(request.Class));
        PartialClass? partialClassA = request.PartialClassA is not null
            ? EnumParser.Parse<PartialClass>(request.PartialClassA, nameof(request.PartialClassA))
            : null;
        PartialClass? partialClassB = request.PartialClassB is not null
            ? EnumParser.Parse<PartialClass>(request.PartialClassB, nameof(request.PartialClassB))
            : null;

        var attributeScores = request.Attributes.ToDictionary(
            kvp => EnumParser.Parse<AttributeName>(kvp.Key, nameof(request.Attributes)),
            kvp => kvp.Value);

        var createdCharacter = Character.Create(request.Name, characterClass, attributeScores,
            userId, request.Background, request.Origin, partialClassA, partialClassB, request.MaxHitPoints);

        await characterRepository.AddAsync(createdCharacter, cancellationToken);
        return createdCharacter.Id;
    }

    public async Task<CharacterDetailDto> UpdateAttributeAsync(
        Guid characterId,
        string userId,
        string attributeString,
        int score,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var attributeName = EnumParser.Parse<AttributeName>(attributeString, nameof(attributeString));
        character.SetAttribute(attributeName, score);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateSkillAsync(
        Guid charId,
        string userId,
        string skillName,
        int rank,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(charId, userId, cancellationToken);
        var skill = EnumParser.Parse<SkillName>(skillName, nameof(skillName));
        character.SetSkillRank(skill, rank);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> AddCustomSkillAsync(
        Guid characterId,
        string userId,
        string name,
        int rank,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.AddCustomSkill(name, rank);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetHpAsync(
        Guid characterId,
        string userId,
        int maxHp,
        int currentHp,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetHitPoints(maxHp, currentHp);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetStrainAsync(
        Guid characterId,
        string userId,
        int currentStrain,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetStrain(currentStrain);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetLevelAsync(
        Guid characterId,
        string userId,
        int level,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetLevel(level);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> AddFocusAsync(
        Guid characterId,
        string userId,
        AddFocusRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focusEffects = request.Effects.Select(e => new FocusEffect(
            EnumParser.Parse<FocusEffectType>(e.Type, nameof(e.Type)),
            e.NumericValue,
            EnumParser.Parse<FocusEffectValueType>(e.ValueType, nameof(e.ValueType)),
            EnumParser.Parse<FocusEffectCondition>(e.Condition, nameof(e.Condition)),
            e.TargetSkill is not null ? EnumParser.Parse<SkillName>(e.TargetSkill, nameof(e.TargetSkill)) : null,
            e.TargetAttribute is not null ? EnumParser.Parse<AttributeName>(e.TargetAttribute, nameof(e.TargetAttribute)) : null,
            e.Description));

        var focus = new Focus(request.Name, request.Level, focusEffects);
        character.AddFocus(focus);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpgradeFocusAsync(
        Guid characterId,
        string userId,
        Guid focusId,
        UpgradeFocusRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focus = character.Foci.FirstOrDefault(f => f.Id == focusId)
            ?? throw new KeyNotFoundException($"Focus {focusId} not found on character {characterId}.");
        focus.UpgradeToLevel2(request.AdditionalEffects.Select(ParseFocusEffect));
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> SetFocusConditionalAsync(
        Guid characterId,
        string userId,
        Guid focusId,
        bool active,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var focus = character.Foci.FirstOrDefault(f => f.Id == focusId)
            ?? throw new KeyNotFoundException($"Focus {focusId} not found on character {characterId}.");
        focus.SetConditionalActive(active);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task RemoveFocusAsync(Guid characterId, string userId, Guid focusId, CancellationToken ct = default)
    {
        var character = await GetOrThrow(characterId, userId, ct);
        character.RemoveFocus(focusId);
        await characterRepository.UpdateAsync(character, ct);
    }

    public async Task<CharacterDetailDto> AddItemAsync(
        Guid characterId,
        string userId,
        AddItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var item = ItemFactory.Create(request);
        character.AddItem(item);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task RemoveItemAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.RemoveItem(itemId);
        await characterRepository.UpdateAsync(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateItemAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        UpdateItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var item = character.Inventory.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            throw new InvalidOperationException($"Item with ID {itemId} not found in character inventory.");

        switch (request.ItemType.ToLower())
        {
            case "weapon":
                if (item is not Weapon weapon)
                    throw new InvalidOperationException($"Item {itemId} is not a weapon.");
                weapon.Update(
                    request.Name,
                    request.Encumbrance,
                    new DamageDie(request.DamageDieCount ?? 1, request.DamageDieSides ?? 6),
                    EnumParser.Parse<AttributeName>(request.AttributeModifier ?? "Strength", nameof(request.AttributeModifier)),
                    EnumParser.Parse<SkillName>(request.CombatSkill ?? "Stab", nameof(request.CombatSkill)),
                    ParseWeaponTags(request.Tags),
                    request is { ShockDamage: not null, ShockAcThreshold: not null }
                        ? new ShockInfo(request.ShockDamage.Value, request.ShockAcThreshold.Value)
                        : null,
                    request.Description);
                break;

            case "armor":
                if (item is not Armor armor)
                    throw new InvalidOperationException($"Item {itemId} is not armor.");
                armor.Update(
                    request.Name,
                    request.Encumbrance,
                    request.AcBonus ?? 0,
                    request.IsShield ?? false,
                    request.Description);
                break;

            default:
                item.Update(request.Name, request.Encumbrance, request.Quantity, request.Description);
                break;
        }

        await characterRepository.UpdateAsync(character, cancellationToken);
        return await MapToDetailDtoAsync(character, cancellationToken);
    }

    private static WeaponTag ParseWeaponTags(string? tags)
    {
        return string.IsNullOrWhiteSpace(tags)
            ? WeaponTag.None
            : EnumParser.Parse<WeaponTag>(tags, nameof(tags));
    }

    public async Task<CharacterDetailDto> ChangeSlotAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        string slotType,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var slot = EnumParser.Parse<ItemSlotType>(slotType, nameof(slotType));
        character.ChangeItemSlot(itemId, slot);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateWeaponAttackConfigAsync(
        Guid characterId,
        string userId,
        Guid itemId,
        string skill,
        string attribute,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        var weapon = character.Inventory.OfType<Weapon>().FirstOrDefault(w => w.Id == itemId);
        if (weapon is null)
            throw new InvalidOperationException($"Weapon with ID {itemId} not found in character inventory.");

        var skillName = EnumParser.Parse<SkillName>(skill, nameof(skill));
        var attrName = EnumParser.Parse<AttributeName>(attribute, nameof(attribute));
        weapon.SetCombatConfig(skillName, attrName);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task<CharacterDetailDto> UpdateNotesAsync(
        Guid characterId,
        string userId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var character = await GetOrThrow(characterId, userId, cancellationToken);
        character.SetNotes(notes);
        await characterRepository.UpdateAsync(character, cancellationToken);
        return await SyncAndMap(character, cancellationToken);
    }

    public async Task DeleteCharacterAsync(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        await characterRepository.DeleteAsync(characterId, userId, cancellationToken);
    }

    private async Task<CharacterDetailDto> SyncAndMap(Character character, CancellationToken cancellationToken)
    {
        var definitions = await focusDefinitionRepository.GetAllAsync(cancellationToken);
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

    private async Task<Character> GetOrThrow(
        Guid characterId,
        string userId,
        CancellationToken cancellationToken)
    {
        return await characterRepository.GetByIdAsync(characterId, userId, cancellationToken)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
    }

    internal async Task<CharacterDetailDto> MapToDetailDtoAsync(
        Character character,
        CancellationToken cancellationToken = default)
    {
        var allAbilities = await classAbilityRepository.GetAllAsync(cancellationToken);
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
            ClassAbilities = classAbilities,
            DerivedStats = calculatedStats
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

    private static FocusEffect ParseFocusEffect(FocusEffectDto e) => new(
        EnumParser.Parse<FocusEffectType>(e.Type, nameof(e.Type)),
        e.NumericValue,
        EnumParser.ParseOrDefault(e.ValueType, FocusEffectValueType.Static),
        EnumParser.ParseOrDefault(e.Condition, FocusEffectCondition.Always),
        e.TargetSkill is not null ? EnumParser.Parse<SkillName>(e.TargetSkill, nameof(e.TargetSkill)) : null,
        e.TargetAttribute is not null ? EnumParser.Parse<AttributeName>(e.TargetAttribute, nameof(e.TargetAttribute)) : null,
        e.Description);

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
