using System;

namespace TournamentManagerTask.Api.DTOs;

public class FinishMatchRequest
{
    public string Result { get; set; } = string.Empty;
    public Guid? WinningTeamId { get; set; }
}
