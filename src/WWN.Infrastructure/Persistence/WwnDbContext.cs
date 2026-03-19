using Microsoft.EntityFrameworkCore;
using WWN.Domain.Aggregates;

namespace WWN.Infrastructure.Persistence;

public class WwnDbContext : DbContext
{
    public DbSet<Character> Characters => Set<Character>();

    public WwnDbContext(DbContextOptions<WwnDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WwnDbContext).Assembly);
    }
}
