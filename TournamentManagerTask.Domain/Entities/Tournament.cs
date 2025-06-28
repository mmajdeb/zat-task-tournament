using TournamentManagerTask.Domain.Exceptions;

namespace TournamentManagerTask.Domain.Entities;

public class Tournament
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public List<Match> Matches { get; private set; } = new();
    public List<string> Teams { get; private set; } = new();

    private Tournament(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tournament name cannot be empty");
        Name = name;
    }

    // Constructor for reconstruction from persistence - internal access only
    internal Tournament(Guid id, string name, List<string> teams, List<Match> matches)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tournament name cannot be empty");

        Id = id;
        Name = name;
        Teams = teams;
        Matches = matches;
    }

    public static Tournament Create(string name, int teamsCount)
    {
        if (teamsCount < 2) throw new DomainValidationException("At least 2 teams are required");

        var tournament = new Tournament(name);

        // Generate team names automatically based on count
        tournament.Teams = new List<string>();
        for (int i = 1; i <= teamsCount; i++)
        {
            tournament.Teams.Add($"Team {i}");
        }

        tournament.Matches = GenerateBracket(tournament.Teams);

        return tournament;
    }
    private static List<Match> GenerateBracket(List<string> teams)
    {
        int teamCount = teams.Count;
        int bracketSize = 1;
        while (bracketSize < teamCount) bracketSize <<= 1;

        int totalRounds = (int)Math.Log2(bracketSize);
        var matchesByRound = new List<List<Match>>();

        for (int r = 1; r <= totalRounds; r++)
            matchesByRound.Add(new List<Match>());

        // Create all matches for each round
        for (int round = 1; round <= totalRounds; round++)
        {
            // matchCount is bracketSize / 2^round
            int matchCount = bracketSize >> round;
            for (int i = 0; i < matchCount; i++)
            {
                matchesByRound[round - 1].Add(new Match(round, null, null));
            }
        }

        // Wire matches to next round
        for (int round = 0; round < totalRounds - 1; round++)
        {
            for (int i = 0; i < matchesByRound[round].Count; i += 2)
            {
                var matchA = matchesByRound[round][i];
                var matchB = matchesByRound[round][i + 1];
                var parentMatch = matchesByRound[round + 1][i / 2];

                matchA.NextMatch = parentMatch;
                matchA.IsTeamAInNextMatchSlot = true;

                matchB.NextMatch = parentMatch;
                matchB.IsTeamAInNextMatchSlot = false;
            }
        }

        // Assign teams to first round with byes
        var firstRound = matchesByRound[0];
        for (int i = 0; i < teamCount; i++)
        {
            var match = firstRound[i / 2];
            if (i % 2 == 0) match.AssignTeamA(teams[i]);
            else match.AssignTeamB(teams[i]);
        }

        return matchesByRound.SelectMany(r => r).OrderBy(m => m.Round).ToList();
    }
}