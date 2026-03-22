namespace WWN.Domain.Entities;

public class KnownSpell
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public Guid SpellId { get; private set; }
    public Spell Spell { get; private set; } = null!;

    public KnownSpell(Guid spellId)
    {
        if (spellId == Guid.Empty)
            throw new ArgumentException("SpellId is required.", nameof(spellId));

        Id = Guid.NewGuid();
        SpellId = spellId;
    }

    private KnownSpell() { } // EF Core
}
