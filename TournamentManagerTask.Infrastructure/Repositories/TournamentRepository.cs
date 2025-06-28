using Microsoft.EntityFrameworkCore;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Entities;
using TournamentManagerTask.Domain.Enums;
using TournamentManagerTask.Infrastructure.Data;
using TournamentManagerTask.Infrastructure.Entities;

namespace TournamentManagerTask.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentDbContext _context;

    public TournamentRepository(TournamentDbContext context)
    {
        _context = context;
    }
    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Tournaments
            .Include(t => t.Teams)
            .Include(t => t.Matches)
            .ThenInclude(m => m.TeamA)
            .Include(t => t.Matches)
            .ThenInclude(m => m.TeamB)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Winner)
            .FirstOrDefaultAsync(t => t.Id == id);

        return MapToDomain(entity);
    }

    public async Task<Tournament?> FindByMatchIdAsync(Guid matchId)
    {
        var entity = await _context.Tournaments
            .Include(t => t.Teams)
            .Include(t => t.Matches)
            .ThenInclude(m => m.TeamA)
            .Include(t => t.Matches)
            .ThenInclude(m => m.TeamB)
            .Include(t => t.Matches)
            .ThenInclude(m => m.Winner)
            .FirstOrDefaultAsync(t => t.Matches.Any(m => m.Id == matchId));

        return MapToDomain(entity);
    }
    public async Task<Tournament> CreateAsync(Tournament domain)
    {
        var entity = MapToEntity(domain);
        _context.Tournaments.Add(entity);
        await _context.SaveChangesAsync();
        return MapToDomain(entity)!;
    }

    public async Task UpdateMatchResultAsync(Guid matchId, string state, Guid? winnerId)
    {
        var matchEntity = await _context.Matches.FirstAsync(m => m.Id == matchId);
        matchEntity.State = state;
        matchEntity.WinnerId = winnerId;

        await _context.SaveChangesAsync();
    }

    private Tournament? MapToDomain(TournamentEntity? entity)
    {
        if (entity == null) return null;

        // Create teams dictionary for quick lookup during match mapping
        var teamLookup = entity.Teams.ToDictionary(t => t.Id, t => new Team(t.Id, t.Name));

        // Map matches
        var matches = entity.Matches.Select(m =>
        {
            var teamA = m.TeamAId.HasValue && teamLookup.ContainsKey(m.TeamAId.Value)
                ? teamLookup[m.TeamAId.Value]
                : null;
            var teamB = m.TeamBId.HasValue && teamLookup.ContainsKey(m.TeamBId.Value)
                ? teamLookup[m.TeamBId.Value]
                : null;
            var winner = m.WinnerId.HasValue && teamLookup.ContainsKey(m.WinnerId.Value)
                ? teamLookup[m.WinnerId.Value]
                : null;
            return new Match(
                    m.Id,
                    m.Round,
                    teamA,
                    teamB,
                    Enum.Parse<MatchState>(m.State),
                    winner
                );
        }).ToList();

        return new Tournament(
            entity.Id,
            entity.Name,
            teamLookup.Values.ToList(),
            matches
        );
    }

    private TournamentEntity MapToEntity(Tournament domain)
    {
        var entity = new TournamentEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            Teams = domain.Teams.Select(t => new TeamEntity { Id = t.Id, Name = t.Name, TournamentId = domain.Id }).ToList(),
            Matches = domain.Matches.Select(m => new MatchEntity
            {
                Id = m.Id,
                Round = m.Round,
                State = m.State.ToString(),
                TeamAId = m.TeamA?.Id,
                TeamBId = m.TeamB?.Id,
                WinnerId = m.Winner?.Id,
                TournamentId = domain.Id
            }).ToList()
        };
        return entity;
    }
}