namespace TournamentManagerTask.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested match is not found
/// </summary>
public class MatchNotFoundException : ApplicationException
{
    public Guid MatchId { get; }

    public MatchNotFoundException(Guid matchId)
        : base($"Match with ID '{matchId}' was not found.")
    {
        MatchId = matchId;
    }

    public MatchNotFoundException(Guid matchId, Exception innerException)
        : base($"Match with ID '{matchId}' was not found.", innerException)
    {
        MatchId = matchId;
    }
}
