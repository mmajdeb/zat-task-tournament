namespace TournamentManagerTask.Domain.Entities;

public class Team
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    public Team(string name)
    {
        Name = name;
    }
}
