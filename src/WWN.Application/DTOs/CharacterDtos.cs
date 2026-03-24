namespace WWN.Application.DTOs;

public record CharacterSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Class { get; init; } = string.Empty;
    public int Level { get; init; }
    public int CurrentHitPoints { get; init; }
    public int MaxHitPoints { get; init; }
}

public record CharacterDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Background { get; init; }
    public string? Origin { get; init; }
    public string Class { get; init; } = string.Empty;
    public string? PartialClassA { get; init; }
    public string? PartialClassB { get; init; }
    public int Level { get; init; }
    public int MaxHitPoints { get; init; }
    public int CurrentHitPoints { get; init; }
    public int ExperiencePoints { get; init; }
    public List<AttributeDto> Attributes { get; init; } = new();
    public List<SkillDto> Skills { get; init; } = new();
    public List<FocusDto> Foci { get; init; } = new();
    public List<ItemDto> Inventory { get; init; } = new();
    public List<KnownSpellDto> Spellbook { get; init; } = new();
    public SpellSlotInfoDto? SpellSlots { get; init; }
    public DerivedStatsDto DerivedStats { get; init; } = null!;
    public string? Notes { get; init; }
}

public record AttributeDto
{
    public string Name { get; init; } = string.Empty;
    public int Score { get; init; }
    public int Modifier { get; init; }
}

public record SkillDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? CustomName { get; init; }
    public int Level { get; init; }
}

public record FocusDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Level { get; init; }
    public List<FocusEffectDto> Effects { get; init; } = new();
}

public record FocusEffectDto
{
    public string Type { get; init; } = string.Empty;
    public int NumericValue { get; init; }
    public string? TargetSkill { get; init; }
    public string? TargetAttribute { get; init; }
    public string? Description { get; init; }
}

public record ItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Encumbrance { get; init; }
    public string SlotType { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string ItemType { get; init; } = "Item";

    // Weapon fields
    public string? DamageDie { get; init; }
    public string? AttributeModifier { get; init; }
    public string? CombatSkill { get; init; }
    public int? ShockDamage { get; init; }
    public int? ShockAcThreshold { get; init; }
    public bool? IsArmorPiercing { get; init; }
    public string? Tags { get; init; }
    public int? AttackBonus { get; init; }

    // Armor fields
    public int? AcBonus { get; init; }
    public bool? IsShield { get; init; }
}

public record FocusDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Level1Description { get; init; } = string.Empty;
    public string? Level2Description { get; init; }
    public bool HasLevel2 { get; init; }
    public bool CanTakeMultipleTimes { get; init; }
}

public record SpellDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record KnownSpellDto
{
    public Guid Id { get; init; }
    public Guid SpellId { get; init; }
    public SpellDto Spell { get; init; } = null!;
}

public record SpellSlotInfoDto
{
    public int[] Available { get; init; } = new int[6];
    public int[] Used { get; init; } = new int[6];
}
