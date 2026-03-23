using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence;

public class WwnDbContext(DbContextOptions<WwnDbContext> options) : DbContext(options)
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<FocusDefinition> FocusDefinitions => Set<FocusDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WwnDbContext).Assembly);
    }
}
