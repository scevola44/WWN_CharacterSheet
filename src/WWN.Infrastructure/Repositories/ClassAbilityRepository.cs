using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class ClassAbilityRepository(WwnDbContext dbContext) : IClassAbilityRepository
{
    public async Task<IReadOnlyList<ClassAbilityDefinition>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.ClassAbilityDefinitions.ToListAsync(ct);

    public async Task AddAsync(ClassAbilityDefinition ability, CancellationToken ct = default)
    {
        await dbContext.ClassAbilityDefinitions.AddAsync(ability, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.ClassAbilityDefinitions.FindAsync([id], ct);
        if (entity is not null)
        {
            dbContext.ClassAbilityDefinitions.Remove(entity);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default)
        => await dbContext.ClassAbilityDefinitions.AnyAsync(ct);

    public async Task<bool> AnyGlobalAsync(CancellationToken ct = default)
        => await dbContext.ClassAbilityDefinitions.AnyAsync(a => a.OwnerId == null, ct);

    public async Task<IReadOnlyList<ClassAbilityDefinition>> GetGlobalAsync(CancellationToken ct = default)
        => await dbContext.ClassAbilityDefinitions.Where(a => a.OwnerId == null).ToListAsync(ct);
}
