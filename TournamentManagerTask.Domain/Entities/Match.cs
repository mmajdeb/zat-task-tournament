using TournamentManagerTask.Domain.Enums;
using TournamentManagerTask.Domain.Exceptions;

namespace TournamentManagerTask.Domain.Entities;

public class Match
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? TeamA { get; private set; }
    public string? TeamB { get; private set; }
    public int Round { get; private set; }
    public MatchState State { get; private set; } = MatchState.Pending;
    public string? Winner { get; private set; }

    public Match? NextMatch { get; set; }
    public bool IsTeamAInNextMatchSlot { get; set; }

    public Match(int round, string? teamA, string? teamB)
    {
        Round = round;
        TeamA = teamA;
        TeamB = teamB;
    }

    // Constructor for reconstruction from persistence - internal access only
    internal Match(Guid id, int round, string? teamA, string? teamB, MatchState state, string? winner)
    {
        Id = id;
        Round = round;
        TeamA = teamA;
        TeamB = teamB;
        State = state;
        Winner = winner;
    }

    public void Finish(FinishResult result, string? winningTeam = null)
    {
        if (State == MatchState.Finished)
            throw new InvalidDomainOperationException("Match already finished");

        switch (result)
        {
            case FinishResult.Winner:
            case FinishResult.WithdrawOne:
                if (winningTeam == null || (winningTeam != TeamA && winningTeam != TeamB))
                    throw new DomainValidationException("Invalid winner");
                Winner = winningTeam;
                break;

            case FinishResult.WithdrawBoth:
                Winner = null;
                break;
        }

        State = MatchState.Finished;

        if (NextMatch != null && Winner != null)
        {
            if (IsTeamAInNextMatchSlot)
                NextMatch.AssignTeamA(Winner);
            else
                NextMatch.AssignTeamB(Winner);
        }
        // Handle WithdrawBoth edge case: if both teams withdraw, and next match has only one team, auto-finish next match
        if (result == FinishResult.WithdrawBoth && NextMatch != null)
        {
            var automaticallyWinningTeam = (!string.IsNullOrEmpty(NextMatch.TeamA) && string.IsNullOrEmpty(NextMatch.TeamB)) ? NextMatch.TeamA :
                               (string.IsNullOrEmpty(NextMatch.TeamA) && !string.IsNullOrEmpty(NextMatch.TeamB)) ? NextMatch.TeamB : null;

            if (!string.IsNullOrEmpty(automaticallyWinningTeam) && NextMatch.State != MatchState.Finished)
            {
                NextMatch.Finish(FinishResult.WithdrawOne, automaticallyWinningTeam);
            }
        }
    }

    public void AssignTeamA(string team) => TeamA = team;
    public void AssignTeamB(string team) => TeamB = team;
}