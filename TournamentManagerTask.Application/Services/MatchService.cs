using Microsoft.Extensions.Logging;
using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Exceptions;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Enums;

namespace TournamentManagerTask.Application.Services;

public class MatchService : IMatchService
{
    private readonly ITournamentRepository _repository;
    private readonly ILogger<MatchService> _logger;

    public MatchService(ITournamentRepository repository, ILogger<MatchService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task FinishMatchAsync(Guid matchId, FinishResultDto input)
    {
        _logger.LogInformation("Starting to finish match {MatchId} with result: {Result}, winning team id: {WinningTeamId}",
            matchId, input.Result, input.WinningTeamId);

        _logger.LogDebug("Finding tournament for match {MatchId}", matchId);
        var tournament = await _repository.FindByMatchIdAsync(matchId);
        if (tournament == null)
        {
            _logger.LogWarning("No tournament found containing match {MatchId}", matchId);
            throw new MatchNotFoundException(matchId);
        }

        _logger.LogDebug("Tournament found: {TournamentName} (ID: {TournamentId}) for match {MatchId}",
            tournament.Name, tournament.Id, matchId);

        var match = tournament.Matches.First(m => m.Id == matchId);

        _logger.LogDebug("Match found: {TeamA} vs {TeamB}, current state: {CurrentState}",
            match.TeamA, match.TeamB, match.State);

        FinishResult result = input.Result switch
        {
            "Winner" => FinishResult.Winner,
            "WithdrawOne" => FinishResult.WithdrawOne,
            "WithdrawBoth" => FinishResult.WithdrawBoth,
            _ => throw new InvalidInputException($"Invalid result type: '{input.Result}'. Valid values are: Winner, WithdrawOne, WithdrawBoth", nameof(input.Result))
        };

        _logger.LogDebug("Result type parsed successfully: {ResultType}", result);

        string? winner = !string.IsNullOrEmpty(input.WinningTeamId)
            ? tournament.Teams.FirstOrDefault(t => t == input.WinningTeamId)
            : null;

        if (!string.IsNullOrEmpty(input.WinningTeamId) && winner == null)
        {
            _logger.LogWarning("Winning team id '{WinningTeamId}' not found in tournament teams", input.WinningTeamId);
        }

        _logger.LogDebug("Finishing match with winner: {Winner}", winner ?? "No winner");
        match.Finish(result, winner);

        // Handle Winner case : if the other match was already finished WithdrawBoth
        if (result == FinishResult.Winner && match.NextMatch != null)
        {
            _logger.LogDebug("Checking for other matches in the same round with the same next match");
            // find the other match in the same round which has the same next match
            var otherMatch = tournament.Matches
                .FirstOrDefault(m => m.Round == match.Round && m.Id != match.Id && m.NextMatch?.Id == match.NextMatch?.Id);

            // If the other match is found and it is finished with no winner, we can apply the winning to the next match
            if (otherMatch != null && otherMatch.State == MatchState.Finished && otherMatch.Winner == null)
            {
                _logger.LogDebug("Other match {OtherMatchId} in the same round is already finished, applying winning", otherMatch.Id);
                // finish next match with winner the winner in the current match
                match.NextMatch.Finish(FinishResult.Winner, match.Winner);
                _logger.LogDebug("Next match {NextMatchId} finished with winner: {Winner}",
                    match.NextMatch.Id, match.Winner);
            }
        }

        _logger.LogDebug("Updating match result in repository for match {MatchId}", matchId);
        await _repository.UpdateMatchResultAsync(matchId, match.State.ToString(), match.Winner);

        var nextMatch = match.NextMatch;
        while (nextMatch != null)
        {
            _logger.LogDebug("Updating next match {NextMatchId} with teams: {TeamA} vs {TeamB} and result: {Result}",
                nextMatch.Id, nextMatch.TeamA, nextMatch.TeamB, nextMatch.State);
            await _repository.UpdateNextMatchAsync(nextMatch.Id, nextMatch.TeamA, nextMatch.TeamB, nextMatch.Winner, nextMatch.State);
            nextMatch = nextMatch.NextMatch;
        }

        _logger.LogInformation("Match {MatchId} finished successfully with winner: {Winner}",
            matchId, match.Winner ?? "No winner");
    }
}
