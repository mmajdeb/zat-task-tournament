namespace TournamentManagerTask.Infrastructure.Entities;

public class MatchEntity
{
    public Guid Id { get; set; }
    public Guid? TeamAId { get; set; }
    public Guid? TeamBId { get; set; }
    public TeamEntity? TeamA { get; set; }
    public TeamEntity? TeamB { get; set; }

    public int Round { get; set; }
    public string State { get; set; } = "Pending";
    public Guid? WinnerId { get; set; }
    public TeamEntity? Winner { get; set; }

    public Guid TournamentId { get; set; }
    public TournamentEntity Tournament { get; set; } = null!;
}
