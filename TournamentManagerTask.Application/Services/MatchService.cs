using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Exceptions;
using TournamentManagerTask.Application.Interfaces;
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
            throw new MatchNotFoundException(matchId);

        var match = tournament.Matches.First(m => m.Id == matchId); FinishResult result = input.Result switch
        {
            "Winner" => FinishResult.Winner,
            "WithdrawOne" => FinishResult.WithdrawOne,
            "WithdrawBoth" => FinishResult.WithdrawBoth,
            _ => throw new InvalidInputException($"Invalid result type: '{input.Result}'. Valid values are: Winner, WithdrawOne, WithdrawBoth", nameof(input.Result))
        };

        string? winner = !string.IsNullOrEmpty(input.WinningTeam)
            ? tournament.Teams.FirstOrDefault(t => t == input.WinningTeam)
            : null;

        match.Finish(result, winner);

        await _repository.UpdateMatchResultAsync(matchId, match.State.ToString(), match.Winner);

        if (match.NextMatch != null)
            await _repository.UpdateNextMatchTeamsAsync(match.NextMatch.Id, match.NextMatch.TeamA, match.NextMatch.TeamB);
    }
}
