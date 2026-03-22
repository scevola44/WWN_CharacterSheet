using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;

namespace WWN.Infrastructure.Persistence;

public class WwnDbContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<FocusDefinition> FocusDefinitions => Set<FocusDefinition>();

    public WwnDbContext(DbContextOptions<WwnDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WwnDbContext).Assembly);
    }
}
