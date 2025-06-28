using TournamentManagerTask.Domain.Enums;

namespace TournamentManagerTask.Application.DTOs;

public class MatchDto
{
    public Guid Id { get; set; }
    public string? TeamA { get; set; }
    public string? TeamB { get; set; }
    public string State { get; set; } = MatchState.Pending.ToString();
    public string? Winner { get; set; }
    public int Round { get; set; }
    public Guid? NextMatchId { get; set; }
}
