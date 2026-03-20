using WWN.Application.DTOs;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Domain.Enums;
using WWN.Domain.Interfaces;
using WWN.Domain.ValueObjects;

namespace WWN.Application.Services;

public class CharacterService
{
    private readonly ICharacterRepository _repo;
    private readonly CharacterSheetCalculator _calc;

    public CharacterService(ICharacterRepository repo, CharacterSheetCalculator calc)
    {
        _repo = repo;
        _calc = calc;
    }

    public async Task<CharacterDetailDto?> GetCharacterAsync(Guid id, CancellationToken ct = default)
    {
        var character = await _repo.GetByIdAsync(id, ct);
        return character is null ? null : MapToDetailDto(character);
    }

    public async Task<IReadOnlyList<CharacterSummaryDto>> ListCharactersAsync(CancellationToken ct = default)
    {
        var characters = await _repo.GetAllAsync(ct);
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

    public async Task<Guid> CreateCharacterAsync(CreateCharacterRequest req, CancellationToken ct = default)
    {
        var charClass = Enum.Parse<CharacterClass>(req.Class, true);
        PartialClass? partialA = req.PartialClassA is not null
            ? Enum.Parse<PartialClass>(req.PartialClassA, true)
            : null;
        PartialClass? partialB = req.PartialClassB is not null
            ? Enum.Parse<PartialClass>(req.PartialClassB, true)
            : null;

        var scores = req.Attributes.ToDictionary(
            kvp => Enum.Parse<AttributeName>(kvp.Key, true),
            kvp => kvp.Value);

        var character = Character.Create(req.Name, charClass, scores,
            req.Background, req.Origin, partialA, partialB, req.MaxHitPoints);

        await _repo.AddAsync(character, ct);
        return character.Id;
    }

    public async Task<CharacterDetailDto> UpdateAttributeAsync(Guid charId, string attrName, int score,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        var attr = Enum.Parse<AttributeName>(attrName, true);
        character.SetAttribute(attr, score);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> UpdateSkillAsync(Guid charId, string skillName, int rank,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        var skill = Enum.Parse<SkillName>(skillName, true);
        character.SetSkillRank(skill, rank);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> AddCustomSkillAsync(Guid charId, string name, int rank,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.AddCustomSkill(name, rank);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> SetHpAsync(Guid charId, int max, int current,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.SetHitPoints(max, current);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> SetLevelAsync(Guid charId, int level,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.SetLevel(level);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> AddFocusAsync(Guid charId, AddFocusRequest req,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        var effects = req.Effects.Select(e => new FocusEffect(
            Enum.Parse<FocusEffectType>(e.Type, true),
            e.NumericValue,
            e.TargetSkill is not null ? Enum.Parse<SkillName>(e.TargetSkill, true) : null,
            e.TargetAttribute is not null ? Enum.Parse<AttributeName>(e.TargetAttribute, true) : null,
            e.Description));

        var focus = new Focus(req.Name, req.Level, effects);
        character.AddFocus(focus);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task RemoveFocusAsync(Guid charId, Guid focusId, CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.RemoveFocus(focusId);
        await _repo.UpdateAsync(character, ct);
    }

    public async Task<CharacterDetailDto> AddItemAsync(Guid charId, AddItemRequest req,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        var slotType = Enum.Parse<ItemSlotType>(req.SlotType, true);

        Item item = req.ItemType.ToLower() switch
        {
            "weapon" => new Weapon(
                req.Name,
                req.Encumbrance,
                new DamageDie(req.DamageDieCount ?? 1, req.DamageDieSides ?? 6),
                Enum.Parse<AttributeName>(req.AttributeModifier ?? "Strength", true),
                ParseWeaponTags(req.Tags),
                req.ShockDamage.HasValue && req.ShockAcThreshold.HasValue
                    ? new ShockInfo(req.ShockDamage.Value, req.ShockAcThreshold.Value)
                    : null,
                slotType,
                req.Description),
            "armor" => new Armor(
                req.Name,
                req.Encumbrance,
                req.AcBonus ?? 0,
                req.IsShield ?? false,
                slotType,
                req.Description),
            _ => new Item(req.Name, req.Encumbrance, slotType, req.Quantity, req.Description)
        };

        character.AddItem(item);
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task RemoveItemAsync(Guid charId, Guid itemId, CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.RemoveItem(itemId);
        await _repo.UpdateAsync(character, ct);
    }

    public async Task<CharacterDetailDto> ChangeSlotAsync(Guid charId, Guid itemId, string slotType,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        var slot = Enum.Parse<ItemSlotType>(slotType, true);
        var item = character.Inventory.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item not found.");
        item.SlotType = slot;
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task<CharacterDetailDto> UpdateNotesAsync(Guid charId, string? notes,
        CancellationToken ct = default)
    {
        var character = await GetOrThrow(charId, ct);
        character.Notes = notes;
        await _repo.UpdateAsync(character, ct);
        return MapToDetailDto(character);
    }

    public async Task DeleteCharacterAsync(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
    }

    private async Task<Character> GetOrThrow(Guid id, CancellationToken ct)
    {
        return await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Character {id} not found.");
    }

    private CharacterDetailDto MapToDetailDto(Character c)
    {
        var derived = _calc.Calculate(c);
        return new CharacterDetailDto
        {
            Id = c.Id,
            Name = c.Name,
            Background = c.Background,
            Origin = c.Origin,
            Class = c.Class.ToString(),
            PartialClassA = c.PartialClassA?.ToString(),
            PartialClassB = c.PartialClassB?.ToString(),
            Level = c.Level,
            MaxHitPoints = c.MaxHitPoints,
            CurrentHitPoints = c.CurrentHitPoints,
            ExperiencePoints = c.ExperiencePoints,
            Notes = c.Notes,
            Attributes = c.Attributes.Select(a => new AttributeDto
            {
                Name = a.Name.ToString(),
                Score = a.Score.Value,
                Modifier = a.Modifier
            }).ToList(),
            Skills = c.Skills.Select(s => new SkillDto
            {
                Id = s.Id,
                Name = s.Name.ToString(),
                CustomName = s.CustomName,
                Level = s.Rank.Level
            }).ToList(),
            Foci = c.Foci.Select(f => new FocusDto
            {
                Id = f.Id,
                Name = f.Name,
                Level = f.Level,
                Effects = f.Effects.Select(e => new FocusEffectDto
                {
                    Type = e.Type.ToString(),
                    NumericValue = e.NumericValue,
                    TargetSkill = e.TargetSkill?.ToString(),
                    TargetAttribute = e.TargetAttribute?.ToString(),
                    Description = e.Description
                }).ToList()
            }).ToList(),
            Inventory = c.Inventory.Select(MapItemDto).ToList(),
            DerivedStats = derived
        };
    }

    private ItemDto MapItemDto(Item item)
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

        if (item is Weapon w)
        {
            dto = dto with
            {
                ItemType = "Weapon",
                DamageDie = w.DamageDie.ToString(),
                AttributeModifier = w.AttributeModifier.ToString(),
                ShockDamage = w.Shock?.Damage,
                ShockAcThreshold = w.Shock?.AcThreshold,
                Tags = w.Tags.ToString()
            };
        }
        else if (item is Armor a)
        {
            dto = dto with
            {
                ItemType = "Armor",
                AcBonus = a.AcBonus,
                IsShield = a.IsShield
            };
        }

        return dto;
    }

    private static WeaponTag ParseWeaponTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags)) return WeaponTag.None;
        return Enum.Parse<WeaponTag>(tags, true);
    }
}
