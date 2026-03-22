using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface IFocusDefinitionRepository
{
    Task<FocusDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<FocusDefinition>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(FocusDefinition fd, CancellationToken ct = default);
    Task UpdateAsync(FocusDefinition fd, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> AnyAsync(CancellationToken ct = default);
}
