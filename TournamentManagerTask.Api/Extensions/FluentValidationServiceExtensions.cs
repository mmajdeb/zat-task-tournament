using Microsoft.Extensions.DependencyInjection;
using TournamentManagerTask.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace TournamentManagerTask.Api.Extensions;

public static class FluentValidationServiceExtensions
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateTournamentValidator>();

        return services;
    }
}
