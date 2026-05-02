using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;
using WWN.Domain.Entities;
using WWN.Infrastructure.Identity;

namespace WWN.Infrastructure.Persistence;

public class WwnDbContext(DbContextOptions<WwnDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<FocusDefinition> FocusDefinitions => Set<FocusDefinition>();
    public DbSet<ClassAbilityDefinition> ClassAbilityDefinitions => Set<ClassAbilityDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WwnDbContext).Assembly);
    }
}
