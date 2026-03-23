namespace WWN.Domain.Enums;

// TODO some tags are not used. Shock might be useless because ShockInfo exists, the others should have an effect in the code.
[Flags]
public enum WeaponTag
{
    None = 0,
    Melee = 1,
    Ranged = 2,
    TwoHanded = 4,
    Subtle = 8,
    Shock = 16,
    Long = 32,
    Thrown = 64
}
