using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class ArtRepository(WwnDbContext dbContext) : IArtRepository
{
    public async Task<Art?> GetByIdAsync(Guid artId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Arts.FirstOrDefaultAsync(a => a.Id == artId, cancellationToken);
    }

    public async Task<IReadOnlyList<Art>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Arts
            .OrderBy(a => a.SourceId)
            .ThenBy(a => a.MinLevel)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Art art, CancellationToken cancellationToken = default)
    {
        await dbContext.Arts.AddAsync(art, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Art art, CancellationToken cancellationToken = default)
    {
        dbContext.Arts.Update(art);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid artId, CancellationToken cancellationToken = default)
    {
        var art = await dbContext.Arts.FindAsync(new object[] { artId }, cancellationToken);
        if (art is not null)
        {
            dbContext.Arts.Remove(art);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Arts.AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyWithSourceIdAsync(int sourceId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Arts.AnyAsync(a => a.SourceId == sourceId, cancellationToken);
    }
}
