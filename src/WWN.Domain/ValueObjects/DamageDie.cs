namespace WWN.Domain.ValueObjects;

public sealed record DamageDie(int Count, int Sides)
{
    public override string ToString() => $"{Count}d{Sides}";
}
