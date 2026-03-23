using Microsoft.EntityFrameworkCore;
using WWN.Domain.Entities;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class SpellRepository(WwnDbContext dbContext) : ISpellRepository
{
    public async Task<Spell?> GetByIdAsync(Guid spellId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Spells.FirstOrDefaultAsync(s => s.Id == spellId, cancellationToken);
    }

    public async Task<IReadOnlyList<Spell>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Spells.OrderBy(s => s.SpellLevel).ThenBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Spell spell, CancellationToken cancellationToken = default)
    {
        await dbContext.Spells.AddAsync(spell, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Spell spell, CancellationToken cancellationToken = default)
    {
        dbContext.Spells.Update(spell);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid spellId, CancellationToken cancellationToken = default)
    {
        var spell = await dbContext.Spells.FindAsync(new object[] { spellId }, cancellationToken);
        if (spell is not null)
        {
            dbContext.Spells.Remove(spell);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Spells.AnyAsync(cancellationToken);
    }
}
