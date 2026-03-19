namespace WWN.Domain.Rules;

public static class AttributeModifierTable
{
    public static int GetModifier(int score) => score switch
    {
        3 => -2,
        >= 4 and <= 7 => -1,
        >= 8 and <= 13 => 0,
        >= 14 and <= 17 => +1,
        18 => +2,
        _ => throw new ArgumentOutOfRangeException(nameof(score), score, "Attribute scores must be 3-18.")
    };
}
