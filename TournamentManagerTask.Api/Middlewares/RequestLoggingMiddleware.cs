using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TournamentManagerTask.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log request start
        _logger.LogInformation("Request {RequestId} started: {Method} {Path}",
            requestId,
            context.Request.Method,
            context.Request.Path);

        // Log request details in debug mode
        _logger.LogDebug("Request {RequestId} details: Query={Query}, ContentType={ContentType}",
            requestId,
            context.Request.QueryString,
            context.Request.ContentType);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log request completion
            var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(logLevel,
                "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("Slow request detected {RequestId}: {ElapsedMilliseconds}ms for {Method} {Path}",
                    requestId,
                    stopwatch.ElapsedMilliseconds,
                    context.Request.Method,
                    context.Request.Path);
            }
        }
    }
}
