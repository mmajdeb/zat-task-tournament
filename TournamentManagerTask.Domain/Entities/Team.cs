using TournamentManagerTask.Domain.Exceptions;

namespace TournamentManagerTask.Domain.Entities;

public class Team
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }

    public Team(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Team name cannot be empty");

        Name = name;
    }
}
