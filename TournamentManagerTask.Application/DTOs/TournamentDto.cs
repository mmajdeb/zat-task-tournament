namespace TournamentManagerTask.Application.DTOs;

public class TournamentDto
{
    public string Name { get; set; } = string.Empty;
    public List<MatchDto> Matches { get; set; } = new();
}
