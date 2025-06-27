namespace TournamentManagerTask.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid operation is attempted on a domain entity
/// </summary>
public class InvalidDomainOperationException : DomainException
{
    public InvalidDomainOperationException(string message) : base(message)
    {
    }

    public InvalidDomainOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
