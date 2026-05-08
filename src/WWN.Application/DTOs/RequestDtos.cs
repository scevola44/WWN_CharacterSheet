using System.ComponentModel.DataAnnotations;

namespace WWN.Application.DTOs;

public record CreateCharacterRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Background { get; init; }
    public string? Origin { get; init; }
    [Required]
    public string Class { get; init; } = string.Empty;
    public string? PartialClassA { get; init; }
    public string? PartialClassB { get; init; }
    public Dictionary<string, int> Attributes { get; init; } = new();
    [Range(1, int.MaxValue)]
    public int MaxHitPoints { get; init; } = 1;
    [Range(1, 10)]
    public int Level { get; init; } = 1;
}

public record UpdateAttributeRequest
{
    [Range(3, 18)]
    public int Score { get; init; }
}

public record UpdateSkillRequest
{
    [Range(-1, 4)]
    public int Level { get; init; }
}

public record AddCustomSkillRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Range(-1, 4)]
    public int Level { get; init; }
}

public record SetHpRequest
{
    [Range(1, int.MaxValue)]
    public int MaxHitPoints { get; init; }
    [Range(0, int.MaxValue)]
    public int CurrentHitPoints { get; init; }
}

public record SetStrainRequest
{
    [Range(0, int.MaxValue)]
    public int CurrentStrain { get; init; }
}

public record SetLevelRequest
{
    [Range(1, 10)]
    public int Level { get; init; }
}

public record LevelUpRequest
{
    [Range(0, int.MaxValue)]
    public int HpGain { get; init; }
}

public record AddFocusRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Range(1, 2)]
    public int Level { get; init; }
    public List<FocusEffectDto> Effects { get; init; } = new();
}

public record AddItemRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Range(0, int.MaxValue)]
    public int Encumbrance { get; init; }
    public string SlotType { get; init; } = "Stowed";
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; } = 1;
    public string ItemType { get; init; } = "Item";

    // Weapon fields
    public int? DamageDieCount { get; init; }
    public int? DamageDieSides { get; init; }
    public string? AttributeModifier { get; init; }
    public string? CombatSkill { get; init; }
    public int? ShockDamage { get; init; }
    public int? ShockAcThreshold { get; init; }
    public string? Tags { get; init; }

    // Armor fields
    public int? AcBonus { get; init; }
    public bool? IsShield { get; init; }
}

public record UpdateItemRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Range(0, int.MaxValue)]
    public int Encumbrance { get; init; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; } = 1;
    public string ItemType { get; init; } = "Item";

    // Weapon fields
    public int? DamageDieCount { get; init; }
    public int? DamageDieSides { get; init; }
    public string? AttributeModifier { get; init; }
    public string? CombatSkill { get; init; }
    public int? ShockDamage { get; init; }
    public int? ShockAcThreshold { get; init; }
    public string? Tags { get; init; }

    // Armor fields
    public int? AcBonus { get; init; }
    public bool? IsShield { get; init; }
}

public record ChangeSlotRequest
{
    [Required]
    public string SlotType { get; init; } = string.Empty;
}

public record UpdateNotesRequest
{
    public string? Notes { get; init; }
}

public record UpdateWeaponAttackConfigRequest
{
    [Required]
    public string Skill { get; init; } = string.Empty;
    [Required]
    public string Attribute { get; init; } = string.Empty;
}

public record CreateFocusDefinitionRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Required]
    public string Level1Description { get; init; } = string.Empty;
    public string? Level2Description { get; init; }
    public bool CanTakeMultipleTimes { get; init; }
    public List<FocusEffectDto> Level1Effects { get; init; } = new();
    public List<FocusEffectDto> Level2Effects { get; init; } = new();
}

public record UpdateFocusDefinitionRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Required]
    public string Level1Description { get; init; } = string.Empty;
    public string? Level2Description { get; init; }
    public bool CanTakeMultipleTimes { get; init; }
    public List<FocusEffectDto> Level1Effects { get; init; } = new();
    public List<FocusEffectDto> Level2Effects { get; init; } = new();
}

public record CreateSpellRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Range(1, 6)]
    public int SpellLevel { get; init; }
    [Required]
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record UpdateSpellRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Range(1, 6)]
    public int SpellLevel { get; init; }
    [Required]
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record UseSpellSlotRequest
{
    [Range(1, 6)]
    public int SpellLevel { get; init; }
}

public record CreateArtRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Required]
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
    [Range(1, 10)]
    public int MinLevel { get; init; } = 1;
    public int EffortCost { get; init; }
    [Required]
    public string Source { get; init; } = "Mage";
}

public record UpdateArtRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
    [Required]
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
    [Range(1, 10)]
    public int MinLevel { get; init; } = 1;
    public int EffortCost { get; init; }
    [Required]
    public string Source { get; init; } = "Mage";
}

public record CommitEffortRequest
{
    public int Commitment { get; init; }
    [Range(1, int.MaxValue)]
    public int Amount { get; init; } = 1;
}

public record ReleaseSustainedRequest
{
    [Range(1, int.MaxValue)]
    public int Amount { get; init; } = 1;
}

public record UpgradeFocusRequest
{
    public List<FocusEffectDto> AdditionalEffects { get; init; } = new();
}

public record SetFocusConditionalRequest
{
    public bool Active { get; init; }
}
