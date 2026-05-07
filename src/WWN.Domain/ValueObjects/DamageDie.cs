namespace WWN.Domain.ValueObjects;

public sealed record DamageDie
{
    public int Count { get; }
    public int Sides { get; }

    public DamageDie(int Count, int Sides)
    {
        if (Count < 1)
            throw new ArgumentOutOfRangeException(nameof(Count), "Damage die count must be at least 1.");
        if (Sides < 2)
            throw new ArgumentOutOfRangeException(nameof(Sides), "Damage die sides must be at least 2.");
        this.Count = Count;
        this.Sides = Sides;
    }

    public override string ToString() => $"{Count}d{Sides}";
}
