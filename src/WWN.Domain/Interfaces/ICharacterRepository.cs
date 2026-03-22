using WWN.Domain.Aggregates;

namespace WWN.Domain.Interfaces;

public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Character>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Character>> GetAllSummariesAsync(CancellationToken ct = default);
    Task AddAsync(Character character, CancellationToken ct = default);
    Task UpdateAsync(Character character, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
