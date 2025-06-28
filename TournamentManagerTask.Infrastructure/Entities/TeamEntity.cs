namespace TournamentManagerTask.Infrastructure.Entities;

public class TeamEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid TournamentId { get; set; }
    public TournamentEntity Tournament { get; set; } = null!;
}
