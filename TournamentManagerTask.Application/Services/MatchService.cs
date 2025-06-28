using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Entities;
using TournamentManagerTask.Domain.Enums;

namespace TournamentManagerTask.Application.Services;

public class MatchService : IMatchService
{
    private readonly ITournamentRepository _repository;

    public MatchService(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task FinishMatchAsync(Guid matchId, FinishResultDto input)
    {
        var tournament = await _repository.FindByMatchIdAsync(matchId);
        if (tournament == null)
            throw new InvalidOperationException($"No tournament found containing match with ID {matchId}");

        var match = tournament.Matches.First(m => m.Id == matchId);

        FinishResult result = input.Result switch
        {
            "Winner" => FinishResult.Winner,
            "WithdrawOne" => FinishResult.WithdrawOne,
            "WithdrawBoth" => FinishResult.WithdrawBoth,
            _ => throw new ArgumentException("Invalid result type")
        };

        Team? winner = input.WinningTeamId.HasValue
            ? tournament.Teams.FirstOrDefault(t => t.Id == input.WinningTeamId.Value)
            : null;

        match.Finish(result, winner);

        await _repository.SaveAsync(tournament);
    }
}
