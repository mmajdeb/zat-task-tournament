using Microsoft.Extensions.DependencyInjection;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Application.Services;

namespace TournamentManagerTask.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<IMatchService, MatchService>();

        return services;
    }
}

