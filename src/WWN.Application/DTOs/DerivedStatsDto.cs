using WWN.Domain.Enums;

namespace WWN.Application.DTOs;

public record DerivedStatsDto
{
    public int ArmorClass { get; init; }
    public int BaseAttackBonus { get; init; }
    public int PhysicalSave { get; init; }
    public int EvasionSave { get; init; }
    public int MentalSave { get; init; }
    public Dictionary<string, int> AttributeModifiers { get; init; } = new();
    public Dictionary<Guid, int> WeaponAttackBonuses { get; init; } = new();
    public Dictionary<Guid, int> WeaponDamageBonuses { get; init; } = new();
    public Dictionary<Guid, int> WeaponShockBonuses { get; init; } = new();
    public int HitDieModifier { get; init; }
    public int HpFocusBonus { get; init; }
}
