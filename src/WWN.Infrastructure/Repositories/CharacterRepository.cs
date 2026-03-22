using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly WwnDbContext _db;

    public CharacterRepository(WwnDbContext db)
    {
        _db = db;
    }

    public async Task<Character?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Characters
            .Include(c => c.Attributes)
            .Include(c => c.Skills)
            .Include(c => c.Foci)
            .Include(c => c.Inventory)
            .Include(c => c.Spellbook)
            .ThenInclude(k => k.Spell)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Character>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Characters
            .Include(c => c.Attributes)
            .Include(c => c.Skills)
            .Include(c => c.Foci)
            .Include(c => c.Inventory)
            .Include(c => c.Spellbook)
            .ThenInclude(k => k.Spell)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Character character, CancellationToken ct = default)
    {
        await _db.Characters.AddAsync(character, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Character character, CancellationToken ct = default)
    {
        _db.Characters.Update(character);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var character = await _db.Characters.FindAsync(new object[] { id }, ct);
        if (character is not null)
        {
            _db.Characters.Remove(character);
            await _db.SaveChangesAsync(ct);
        }
    }
}
