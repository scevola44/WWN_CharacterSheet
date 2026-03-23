using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface IFocusDefinitionRepository
{
    Task<FocusDefinition?> GetByIdAsync(Guid focusId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FocusDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(FocusDefinition focusDefinition, CancellationToken cancellationToken = default);
    Task UpdateAsync(FocusDefinition focusDefinition, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid focusId, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
