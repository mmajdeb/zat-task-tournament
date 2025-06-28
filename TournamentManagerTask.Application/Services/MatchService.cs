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
        _logger.LogInformation("Starting to finish match {MatchId} with result: {Result}, winning team: {WinningTeam}",
            matchId, input.Result, input.WinningTeam);

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

        string? winner = !string.IsNullOrEmpty(input.WinningTeam)
            ? tournament.Teams.FirstOrDefault(t => t == input.WinningTeam)
            : null;

        if (!string.IsNullOrEmpty(input.WinningTeam) && winner == null)
        {
            _logger.LogWarning("Winning team '{WinningTeam}' not found in tournament teams", input.WinningTeam);
        }

        _logger.LogDebug("Finishing match with winner: {Winner}", winner ?? "No winner");
        match.Finish(result, winner);

        _logger.LogDebug("Updating match result in repository for match {MatchId}", matchId);
        await _repository.UpdateMatchResultAsync(matchId, match.State.ToString(), match.Winner);

        if (match.NextMatch != null)
        {
            _logger.LogDebug("Updating next match {NextMatchId} with teams: {TeamA} vs {TeamB}",
                match.NextMatch.Id, match.NextMatch.TeamA, match.NextMatch.TeamB);
            await _repository.UpdateNextMatchTeamsAsync(match.NextMatch.Id, match.NextMatch.TeamA, match.NextMatch.TeamB);
        }
        else
        {
            _logger.LogDebug("No next match to update for match {MatchId}", matchId);
        }

        _logger.LogInformation("Match {MatchId} finished successfully with winner: {Winner}",
            matchId, match.Winner ?? "No winner");
    }
}
