using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class FocusDefinitionRepository(WwnDbContext dbContext) : IFocusDefinitionRepository
{
    public async Task<FocusDefinition?> GetByIdAsync(Guid focusId, CancellationToken cancellationToken = default)
        => await dbContext.FocusDefinitions.FirstOrDefaultAsync(f => f.Id == focusId, cancellationToken);

    public async Task<IReadOnlyList<FocusDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.FocusDefinitions.OrderBy(f => f.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(FocusDefinition focusDefinition, CancellationToken cancellationToken = default)
    {
        await dbContext.FocusDefinitions.AddAsync(focusDefinition, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FocusDefinition focusDefinition, CancellationToken cancellationToken = default)
    {
        dbContext.FocusDefinitions.Update(focusDefinition);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid focusId, CancellationToken cancellationToken = default)
    {
        var focusDefinition = await dbContext.FocusDefinitions.FindAsync([focusId], cancellationToken);
        if (focusDefinition is not null)
        {
            dbContext.FocusDefinitions.Remove(focusDefinition);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => await dbContext.FocusDefinitions.AnyAsync(cancellationToken);
}
