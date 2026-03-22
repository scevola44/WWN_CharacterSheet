using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class SpellRepository : ISpellRepository
{
    private readonly WwnDbContext _db;

    public SpellRepository(WwnDbContext db)
    {
        _db = db;
    }

    public async Task<Spell?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Spells.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IReadOnlyList<Spell>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Spells.OrderBy(s => s.SpellLevel).ThenBy(s => s.Name).ToListAsync(ct);
    }

    public async Task AddAsync(Spell spell, CancellationToken ct = default)
    {
        await _db.Spells.AddAsync(spell, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var spell = await _db.Spells.FindAsync(new object[] { id }, ct);
        if (spell is not null)
        {
            _db.Spells.Remove(spell);
            await _db.SaveChangesAsync(ct);
        }
    }
}
