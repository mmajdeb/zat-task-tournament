using Microsoft.EntityFrameworkCore;
using TournamentManagerTask.Infrastructure.Entities;

namespace TournamentManagerTask.Infrastructure.Data;

public class TournamentDbContext : DbContext
{
    public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }

    public DbSet<TournamentEntity> Tournaments => Set<TournamentEntity>();
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();
    public DbSet<MatchEntity> Matches => Set<MatchEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TournamentDbContext).Assembly);
    }
}
