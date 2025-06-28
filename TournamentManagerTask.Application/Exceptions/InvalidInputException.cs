namespace TournamentManagerTask.Application.Exceptions;

/// <summary>
/// Exception thrown when invalid input is provided to application services
/// </summary>
public class InvalidInputException : ApplicationException
{
    public string? ParameterName { get; }

    public InvalidInputException(string message) : base(message)
    {
    }

    public InvalidInputException(string message, string parameterName) : base(message)
    {
        ParameterName = parameterName;
    }

    public InvalidInputException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidInputException(string message, string parameterName, Exception innerException)
        : base(message, innerException)
    {
        ParameterName = parameterName;
    }
}
