namespace WWN.Domain.Enums;

[Flags]
public enum WeaponTag
{
    None = 0,
    TwoHanded = 4,
    Subtle = 8,
    Long = 32,
    Thrown = 64,
    AP = 128,
    Reload = 256
}
