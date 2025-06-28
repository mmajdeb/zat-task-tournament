using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace TournamentManagerTask.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddDefaultLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        return builder;
    }
}
