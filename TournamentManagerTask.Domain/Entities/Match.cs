using TournamentManagerTask.Domain.Enums;

namespace TournamentManagerTask.Domain.Entities;

public class Match
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Team? TeamA { get; private set; }
    public Team? TeamB { get; private set; }
    public int Round { get; private set; }
    public MatchState State { get; private set; } = MatchState.Pending;
    public Team? Winner { get; private set; }

    public Match(int round, Team? teamA, Team? teamB)
    {
        Round = round;
        TeamA = teamA;
        TeamB = teamB;
    }

    public void Finish(FinishResult result, Team? winningTeam = null)
    {
        if (State == MatchState.Finished)
            throw new InvalidOperationException("Match already finished");

        switch (result)
        {
            case FinishResult.Winner:
                if (winningTeam == null || (winningTeam != TeamA && winningTeam != TeamB))
                    throw new ArgumentException("Invalid winner");
                Winner = winningTeam;
                break;

            case FinishResult.WithdrawOne:
                Winner = TeamA != null && TeamB == null ? TeamA : TeamB;
                break;

            case FinishResult.WithdrawBoth:
                Winner = null;
                break;
        }

        State = MatchState.Finished;
    }
}