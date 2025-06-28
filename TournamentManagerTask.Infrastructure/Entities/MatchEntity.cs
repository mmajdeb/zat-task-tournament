namespace TournamentManagerTask.Infrastructure.Entities;

public class MatchEntity
{
    public Guid Id { get; set; }
    public string? TeamA { get; set; }
    public string? TeamB { get; set; }

    public int Round { get; set; }
    public string State { get; set; } = "Pending";
    public string? Winner { get; set; }

    public Guid TournamentId { get; set; }
    public TournamentEntity Tournament { get; set; } = null!;
}
