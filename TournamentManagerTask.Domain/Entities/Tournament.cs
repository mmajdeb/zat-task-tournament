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
        int totalTeams = teams.Count;
        int nextPowerOf2 = 1;
        while (nextPowerOf2 < totalTeams) nextPowerOf2 <<= 1;

        int totalMatches = nextPowerOf2 - 1;
        var matches = new Match[totalMatches];

        // Assign round numbers manually
        int[] matchRounds = CalculateMatchRounds(nextPowerOf2);
        for (int i = 0; i < totalMatches; i++)
        {
            matches[i] = new Match(matchRounds[i], null, null);
        }

        // Link matches
        int offset = 0;
        int roundSize = nextPowerOf2 / 2;
        while (roundSize > 0)
        {
            for (int i = 0; i < roundSize; i++)
            {
                int left = offset + i * 2;
                int right = offset + i * 2 + 1;
                int parent = offset + roundSize + i;

                Console.WriteLine($"Linking matches: left={left}, right={right}, parent={parent}");
                if (parent < totalMatches)
                {
                    matches[left].NextMatch = matches[parent];
                    matches[left].IsTeamAInNextMatchSlot = true;

                    matches[right].NextMatch = matches[parent];
                    matches[right].IsTeamAInNextMatchSlot = false;
                }
            }
            offset += roundSize;
            roundSize /= 2;
        }

        // Assign teams to first round matches
        int firstRoundMatches = nextPowerOf2 / 2;
        for (int i = 0; i < teams.Count; i++)
        {
            int matchIndex = i / 2;
            if (matchIndex < firstRoundMatches)
            {
                var match = matches[matchIndex];
                if (i % 2 == 0)
                    match.AssignTeamA(teams[i]);
                else
                    match.AssignTeamB(teams[i]);
            }
        }

        return matches.ToList();
    }

    private static int[] CalculateMatchRounds(int totalSlots)
    {
        int totalMatches = totalSlots - 1;
        int[] rounds = new int[totalMatches];
        int round = 1;
        int matchesInRound = totalSlots / 2;
        int index = 0;

        while (matchesInRound > 0)
        {
            for (int i = 0; i < matchesInRound; i++)
            {
                rounds[index++] = round;
            }
            round++;
            matchesInRound /= 2;
        }

        return rounds;
    }
}