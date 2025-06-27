using TournamentManagerTask.Domain.Exceptions;

namespace TournamentManagerTask.Domain.Entities;

public class Tournament
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public List<Match> Matches { get; private set; } = new();
    public List<Team> Teams { get; private set; } = new();

    private Tournament(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tournament name cannot be empty");

        Name = name;
    }
    public static Tournament Create(string name, int teamsCount, List<string> teamNames)
    {
        if (teamsCount < 2) throw new DomainValidationException("At least 2 teams are required");

        var tournament = new Tournament(name);
        tournament.Teams = teamNames.Select(name => new Team(name)).ToList();

        return tournament;
    }
}