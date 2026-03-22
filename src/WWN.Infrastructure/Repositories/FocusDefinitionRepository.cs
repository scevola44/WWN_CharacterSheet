using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class FocusDefinitionRepository : IFocusDefinitionRepository
{
    private readonly WwnDbContext _db;

    public FocusDefinitionRepository(WwnDbContext db)
    {
        _db = db;
    }

    public async Task<FocusDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.FocusDefinitions.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<IReadOnlyList<FocusDefinition>> GetAllAsync(CancellationToken ct = default)
        => await _db.FocusDefinitions.OrderBy(f => f.Name).ToListAsync(ct);

    public async Task AddAsync(FocusDefinition fd, CancellationToken ct = default)
    {
        await _db.FocusDefinitions.AddAsync(fd, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FocusDefinition fd, CancellationToken ct = default)
    {
        _db.FocusDefinitions.Update(fd);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var fd = await _db.FocusDefinitions.FindAsync(new object[] { id }, ct);
        if (fd is not null)
        {
            _db.FocusDefinitions.Remove(fd);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default)
        => await _db.FocusDefinitions.AnyAsync(ct);
}
