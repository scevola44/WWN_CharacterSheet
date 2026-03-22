using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface ISpellRepository
{
    Task<Spell?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Spell>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Spell spell, CancellationToken ct = default);
    Task UpdateAsync(Spell spell, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
