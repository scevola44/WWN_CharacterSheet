namespace WWN.Application.DTOs;

public record CreateCharacterRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Background { get; init; }
    public string? Origin { get; init; }
    public string Class { get; init; } = string.Empty;
    public string? PartialClassA { get; init; }
    public string? PartialClassB { get; init; }
    public Dictionary<string, int> Attributes { get; init; } = new();
    public int MaxHitPoints { get; init; } = 1;
}

public record UpdateAttributeRequest
{
    public int Score { get; init; }
}

public record UpdateSkillRequest
{
    public int Level { get; init; }
}

public record AddCustomSkillRequest
{
    public string Name { get; init; } = string.Empty;
    public int Level { get; init; }
}

public record SetHpRequest
{
    public int MaxHitPoints { get; init; }
    public int CurrentHitPoints { get; init; }
}

public record SetLevelRequest
{
    public int Level { get; init; }
}

public record AddFocusRequest
{
    public string Name { get; init; } = string.Empty;
    public int Level { get; init; }
    public List<FocusEffectDto> Effects { get; init; } = new();
}

public record AddItemRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Encumbrance { get; init; }
    public string SlotType { get; init; } = "Stowed";
    public int Quantity { get; init; } = 1;
    public string ItemType { get; init; } = "Item";

    // Weapon fields
    public int? DamageDieCount { get; init; }
    public int? DamageDieSides { get; init; }
    public string? AttributeModifier { get; init; }
    public int? ShockDamage { get; init; }
    public int? ShockAcThreshold { get; init; }
    public string? Tags { get; init; }

    // Armor fields
    public int? AcBonus { get; init; }
    public bool? IsShield { get; init; }
}

public record ChangeSlotRequest
{
    public string SlotType { get; init; } = string.Empty;
}

public record UpdateNotesRequest
{
    public string? Notes { get; init; }
}

public record CreateSpellRequest
{
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record UpdateSpellRequest
{
    public string Name { get; init; } = string.Empty;
    public int SpellLevel { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Summary { get; init; }
}

public record UseSpellSlotRequest
{
    public int SpellLevel { get; init; }
}
