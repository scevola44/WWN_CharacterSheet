using WWN.Domain.Entities;

namespace WWN.Domain.Interfaces;

public interface IClassAbilityRepository
{
    Task<IReadOnlyList<ClassAbilityDefinition>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(ClassAbilityDefinition ability, CancellationToken ct = default);
    Task<bool> AnyAsync(CancellationToken ct = default);
}
