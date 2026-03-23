using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface ISpellRepository
{
    Task<Spell?> GetByIdAsync(Guid spellId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Spell>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Spell spell, CancellationToken cancellationToken = default);
    Task UpdateAsync(Spell spell, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid spellId, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
