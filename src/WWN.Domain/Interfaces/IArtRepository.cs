using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface IArtRepository
{
    Task<Art?> GetByIdAsync(Guid artId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Art>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Art art, CancellationToken cancellationToken = default);
    Task UpdateAsync(Art art, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid artId, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
