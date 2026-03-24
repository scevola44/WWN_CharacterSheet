namespace WWN.Domain.Enums;

public enum FocusEffectCondition
{
    Always,      // always applied
    StabWeapon,  // melee + thrown weapons (Stab skill)
    ShootWeapon, // ranged weapons (Shoot skill)
    PunchWeapon, // unarmed (Punch skill)
    Conditional  // requires ConditionalActive toggle on the character's Focus instance
}
