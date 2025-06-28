namespace TournamentManagerTask.Application.DTOs;

public class FinishResultDto
{
    public string Result { get; set; } = string.Empty;
    public Guid? WinningTeamId { get; set; }
}
