namespace TournamentManagerTask.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested tournament is not found
/// </summary>
public class TournamentNotFoundException : ApplicationException
{
    public Guid TournamentId { get; }

    public TournamentNotFoundException(Guid tournamentId)
        : base($"Tournament with ID '{tournamentId}' was not found.")
    {
        TournamentId = tournamentId;
    }

    public TournamentNotFoundException(Guid tournamentId, Exception innerException)
        : base($"Tournament with ID '{tournamentId}' was not found.", innerException)
    {
        TournamentId = tournamentId;
    }
}
