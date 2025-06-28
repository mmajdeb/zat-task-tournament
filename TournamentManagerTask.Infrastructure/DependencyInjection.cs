using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Infrastructure.Data;
using TournamentManagerTask.Infrastructure.Repositories;

namespace TournamentManagerTask.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register DbContext with InMemory database
        services.AddDbContext<TournamentDbContext>(options =>
            options.UseInMemoryDatabase("TournamentDatabase"));

        // Register repositories
        services.AddScoped<ITournamentRepository, TournamentRepository>();

        return services;
    }
}
