using WWN.Domain.Enums;
using WWN.Domain.ValueObjects;

namespace WWN.Domain.Entities;

public class CharacterAttribute
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public AttributeName Name { get; private set; }
    public AttributeScore Score { get; private set; } = null!;

    public int Modifier => Score.Modifier;

    public CharacterAttribute(AttributeName name, int score)
    {
        Id = Guid.NewGuid();
        Name = name;
        Score = new AttributeScore(score);
    }

    private CharacterAttribute() { } // EF Core

    public void SetScore(int value) => Score = new AttributeScore(value);
}
