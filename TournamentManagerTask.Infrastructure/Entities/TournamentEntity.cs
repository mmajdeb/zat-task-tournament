namespace TournamentManagerTask.Infrastructure.Entities;

public class TournamentEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Teams { get; set; } = new();
    public List<MatchEntity> Matches { get; set; } = new();
}
