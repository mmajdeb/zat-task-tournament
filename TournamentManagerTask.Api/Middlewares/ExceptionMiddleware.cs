using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentValidation;
using TournamentManagerTask.Application.Exceptions;
using TournamentManagerTask.Domain.Exceptions;

namespace TournamentManagerTask.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message, details) = GetExceptionDetails(ex);
        response.StatusCode = statusCode;

        _logger.LogError(ex, "Exception occurred: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            details = details,
            status = statusCode,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }

    private (int statusCode, string message, object details) GetExceptionDetails(Exception ex)
    {
        return ex switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.BadRequest,
                "Validation failed",
                new
                {
                    errors = validationEx.Errors.Select(e => new
                    {
                        propertyName = e.PropertyName,
                        errorMessage = e.ErrorMessage,
                        attemptedValue = e.AttemptedValue
                    })
                }
            ),

            TournamentNotFoundException tournamentNotFound => (
                (int)HttpStatusCode.NotFound,
                tournamentNotFound.Message,
                new { tournamentId = tournamentNotFound.TournamentId }
            ),

            MatchNotFoundException matchNotFound => (
                (int)HttpStatusCode.NotFound,
                matchNotFound.Message,
                new { matchId = matchNotFound.MatchId }
            ),

            InvalidInputException invalidInput => (
                (int)HttpStatusCode.BadRequest,
                invalidInput.Message,
                invalidInput.ParameterName != null ? new { parameterName = invalidInput.ParameterName } : new { }
            ),

            DomainValidationException domainValidation => (
                (int)HttpStatusCode.BadRequest,
                domainValidation.Message,
                new { }
            ),

            InvalidDomainOperationException invalidDomainOperation => (
                (int)HttpStatusCode.BadRequest,
                invalidDomainOperation.Message,
                new { }
            ),

            DomainException domainEx => (
                (int)HttpStatusCode.BadRequest,
                domainEx.Message,
                new { }
            ),

            Application.Exceptions.ApplicationException appEx => (
                (int)HttpStatusCode.BadRequest,
                appEx.Message,
                new { }
            ),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An internal server error occurred",
                new { }
            )
        };
    }
}