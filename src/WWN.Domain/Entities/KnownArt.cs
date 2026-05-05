namespace WWN.Domain.Entities;

public class KnownArt
{
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public Guid ArtId { get; private set; }
    public Art Art { get; private set; } = null!;

    public KnownArt(Guid artId)
    {
        if (artId == Guid.Empty)
            throw new ArgumentException("ArtId is required.", nameof(artId));

        Id = Guid.NewGuid();
        ArtId = artId;
    }

    private KnownArt() { } // EF Core
}
