using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class ArtSourceRepository(WwnDbContext dbContext) : IArtSourceRepository
{
    public async Task<IReadOnlyList<ArtSource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ArtSources
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArtSource?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.ArtSources.FindAsync([id], cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ArtSources.Where(x => x.Code == code);
        if (excludeId.HasValue)
            query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(ArtSource artSource, CancellationToken cancellationToken = default)
    {
        await dbContext.ArtSources.AddAsync(artSource, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ArtSource artSource, CancellationToken cancellationToken = default)
    {
        dbContext.ArtSources.Update(artSource);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var source = await dbContext.ArtSources.FindAsync([id], cancellationToken);
        if (source is not null)
        {
            dbContext.ArtSources.Remove(source);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
