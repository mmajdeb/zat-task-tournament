namespace TournamentManagerTask.Api.DTOs;

public class FinishMatchRequest
{
    public string Result { get; set; } = string.Empty;
    public string WinningTeam { get; set; }
}
