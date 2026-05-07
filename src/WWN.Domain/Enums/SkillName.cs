namespace WWN.Domain.Enums;

public enum SkillName
{
    // Original 16 — ordinals 0-15 must not change (stored as integers in focus/ability JSON)
    Connect,
    Know,
    Lead,
    Magic,
    Notice,
    Perform,
    Pray,
    Punch,
    Ride,
    Sail,
    Shoot,
    Sneak,
    Stab,
    Survive,
    Trade,
    Work,
    // Added skills — appended to preserve existing ordinals
    Administer,
    Convince,
    Craft,
    Exert,
    Heal,
    Custom
}
