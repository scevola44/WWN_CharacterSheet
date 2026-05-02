using WWN.Domain.Aggregates;

namespace WWN.Domain.Interfaces;

public interface ICharacterRepository
{
    Task<Character?> GetByIdAsync(Guid characterId, string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Character>> GetAllSummariesAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Character character, CancellationToken cancellationToken = default);
    Task UpdateAsync(Character character, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid characterId, string userId, CancellationToken ct = default);
}
