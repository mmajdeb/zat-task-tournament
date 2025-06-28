using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Entities;
using TournamentManagerTask.Domain.Enums;
using TournamentManagerTask.Infrastructure.Data;
using TournamentManagerTask.Infrastructure.Entities;

namespace TournamentManagerTask.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentDbContext _context;
    private readonly ILogger<TournamentRepository> _logger;

    public TournamentRepository(TournamentDbContext context, ILogger<TournamentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Retrieving tournament by ID: {TournamentId}", id);

        var entity = await _context.Tournaments
            .Include(t => t.Matches)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity == null)
        {
            _logger.LogDebug("Tournament not found with ID: {TournamentId}", id);
            return null;
        }

        _logger.LogDebug("Tournament found: {TournamentName} with {MatchCount} matches",
            entity.Name, entity.Matches.Count);

        return MapToDomain(entity);
    }

    public async Task<Tournament?> FindByMatchIdAsync(Guid matchId)
    {
        _logger.LogDebug("Finding tournament by match ID: {MatchId}", matchId);

        var entity = await _context.Tournaments
            .Include(t => t.Matches)
            .FirstOrDefaultAsync(t => t.Matches.Any(m => m.Id == matchId));

        if (entity == null)
        {
            _logger.LogDebug("No tournament found containing match ID: {MatchId}", matchId);
            return null;
        }

        _logger.LogDebug("Tournament found: {TournamentName} containing match {MatchId}",
            entity.Name, matchId);

        return MapToDomain(entity);
    }

    public async Task<Tournament> CreateAsync(Tournament domain)
    {
        _logger.LogDebug("Creating tournament: {TournamentName} with ID: {TournamentId}",
            domain.Name, domain.Id);

        var entity = MapToEntity(domain);

        _logger.LogDebug("Mapped tournament to entity with {MatchCount} matches",
            entity.Matches.Count);

        _context.Tournaments.Add(entity);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Tournament {TournamentId} successfully saved to database", domain.Id);
        return MapToDomain(entity)!;
    }

    public async Task UpdateMatchResultAsync(Guid matchId, string state, string? winner)
    {
        _logger.LogDebug("Updating match result for match {MatchId}: state={State}, winner={Winner}",
            matchId, state, winner);

        var matchEntity = await _context.Matches.FindAsync(matchId);
        if (matchEntity != null)
        {
            var oldState = matchEntity.State;

            matchEntity.State = state;
            matchEntity.Winner = winner;

            await _context.SaveChangesAsync();

            _logger.LogDebug("Match {MatchId} result updated: {OldState} -> {NewState}, winner: {Winner}",
                matchId, oldState, state, winner);
        }
        else
        {
            _logger.LogWarning("Match not found for update: {MatchId}", matchId);
        }
    }

    public async Task UpdateNextMatchTeamsAsync(Guid matchId, string? teamA, string? teamB)
    {
        _logger.LogDebug("Updating next match teams for match {MatchId}: TeamA={TeamA}, TeamB={TeamB}",
            matchId, teamA, teamB);

        var nextMatch = await _context.Matches.FindAsync(matchId);
        if (nextMatch != null)
        {
            nextMatch.TeamA = teamA;
            nextMatch.TeamB = teamB;

            await _context.SaveChangesAsync();

            _logger.LogDebug("Next match {MatchId} teams updated: ({NewTeamA} : {NewTeamB})",
                matchId, teamA, teamB);
        }
        else
        {
            _logger.LogWarning("Next match not found for update: {MatchId}", matchId);
        }
    }

    private Tournament? MapToDomain(TournamentEntity? entity)
    {
        if (entity == null) return null;

        // Map matches
        var matches = entity.Matches.Select(m => new Match(
            m.Id,
            m.Round,
            m.TeamA,
            m.TeamB,
            Enum.Parse<MatchState>(m.State),
            m.Winner
        )).ToList();

        return new Tournament(
            entity.Id,
            entity.Name,
            entity.Teams,
            matches
        );
    }

    private TournamentEntity MapToEntity(Tournament domain)
    {
        var entity = new TournamentEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Teams = domain.Teams,
            Matches = domain.Matches.Select(m => new MatchEntity
            {
                Id = m.Id,
                Round = m.Round,
                State = m.State.ToString(),
                TeamA = m.TeamA,
                TeamB = m.TeamB,
                Winner = m.Winner,
                TournamentId = domain.Id
            }).ToList()
        };
        return entity;
    }
}