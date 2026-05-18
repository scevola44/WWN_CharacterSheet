namespace WWN.Domain.Enums;

[Flags]
public enum WeaponTag
{
    None = 0,
    TwoHanded = 4,
    Subtle = 8,
    Long = 32,
    Throwable = 64,
    AP = 128,
    Reload = 256,
    Fixed = 512,
    LessLethal = 1024,
    Numerous = 2048,
    PreciselyMurderous = 4096,
    SlowReload = 8192,
    SingleShot = 16384,
}
