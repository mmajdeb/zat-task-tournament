using Microsoft.Extensions.Logging;
using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Exceptions;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Entities;

namespace TournamentManagerTask.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _repository;
    private readonly ILogger<TournamentService> _logger;

    public TournamentService(ITournamentRepository repository, ILogger<TournamentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> CreateTournamentAsync(string name, int teamsCount)
    {
        _logger.LogInformation("Creating tournament: {TournamentName} with {TeamsCount} teams", name, teamsCount);

        var tournament = Tournament.Create(name, teamsCount);

        _logger.LogDebug("Tournament domain entity created with ID: {TournamentId}", tournament.Id);
        _logger.LogDebug("Tournament has {MatchCount} matches generated", tournament.Matches.Count);

        await _repository.CreateAsync(tournament);

        _logger.LogInformation("Tournament {TournamentId} successfully created and persisted", tournament.Id);
        return tournament.Id;
    }

    public async Task<TournamentDto> GetTournamentStateAsync(Guid tournamentId)
    {
        _logger.LogInformation("Retrieving tournament state for ID: {TournamentId}", tournamentId);

        var tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            _logger.LogWarning("Tournament not found with ID: {TournamentId}", tournamentId);
            throw new TournamentNotFoundException(tournamentId);
        }

        _logger.LogDebug("Tournament found: {TournamentName} with {MatchCount} matches",
            tournament.Name, tournament.Matches.Count);

        return new TournamentDto
        {
            Name = tournament.Name,
            Matches = tournament.Matches.Select(m => new MatchDto
            {
                Id = m.Id,
                TeamA = m.TeamA,
                TeamB = m.TeamB,
                Round = m.Round,
                State = m.State.ToString(),
                Winner = m.Winner
            }).ToList()
        };
    }
}
