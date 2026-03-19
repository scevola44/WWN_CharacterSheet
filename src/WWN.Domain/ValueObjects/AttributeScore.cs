using WWN.Domain.Rules;

namespace WWN.Domain.ValueObjects;

public sealed record AttributeScore
{
    public int Value { get; }
    public int Modifier => AttributeModifierTable.GetModifier(Value);

    public AttributeScore(int value)
    {
        if (value < 3 || value > 18)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Attribute scores must be 3-18.");
        Value = value;
    }

    private AttributeScore() { } // EF Core
}
