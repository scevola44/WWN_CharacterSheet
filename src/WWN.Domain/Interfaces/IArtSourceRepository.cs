using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface IArtSourceRepository
{
    Task<IReadOnlyList<ArtSource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ArtSource?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(ArtSource artSource, CancellationToken cancellationToken = default);
    Task UpdateAsync(ArtSource artSource, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
