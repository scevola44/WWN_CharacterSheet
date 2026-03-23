using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;
using WWN.Domain.Interfaces;
using WWN.Infrastructure.Persistence;

namespace WWN.Infrastructure.Repositories;

public class CharacterRepository(WwnDbContext dbContext) : ICharacterRepository
{
    public async Task<Character?> GetByIdAsync(Guid characterId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Characters
            .Include(character => character.Attributes)
            .Include(character => character.Skills)
            .Include(character => character.Foci)
            .Include(character => character.Inventory)
            .Include(character => character.Spellbook)
            .ThenInclude(k => k.Spell)
            .FirstOrDefaultAsync(character => character.Id == characterId, cancellationToken);
    }

    public async Task<IReadOnlyList<Character>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Characters
            .Include(character => character.Attributes)
            .Include(character => character.Skills)
            .Include(character => character.Foci)
            .Include(character => character.Inventory)
            .Include(character => character.Spellbook)
            .ThenInclude(knownSpell => knownSpell.Spell)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Character>> GetAllSummariesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Characters
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Character character, CancellationToken cancellationToken = default)
    {
        await dbContext.Characters.AddAsync(character, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Character character, CancellationToken cancellationToken = default)
    {
        var entry = dbContext.Entry(character);
        if (entry.State == EntityState.Detached)
            dbContext.Characters.Update(character);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid characterId, CancellationToken ct = default)
    {
        var character = await dbContext.Characters.FindAsync([characterId], ct);
        if (character is not null)
        {
            dbContext.Characters.Remove(character);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
