namespace WWN.Domain.Entities;

public class KnownSpell
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public Guid SpellId { get; private set; }
    public Spell Spell { get; private set; } = null!;

    public KnownSpell(Guid spellId, Spell spell)
    {
        if (spellId == Guid.Empty)
            throw new ArgumentException("SpellId is required.", nameof(spellId));
        if (spell == null)
            throw new ArgumentNullException(nameof(spell));

        Id = Guid.NewGuid();
        SpellId = spellId;
        Spell = spell;
    }

    private KnownSpell() { } // EF Core
}
