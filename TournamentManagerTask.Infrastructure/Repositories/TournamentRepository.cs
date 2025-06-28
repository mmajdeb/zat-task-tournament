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
            .Include(t => t.Matches)
            .FirstOrDefaultAsync(t => t.Id == id);

        return MapToDomain(entity);
    }

    public async Task<Tournament?> FindByMatchIdAsync(Guid matchId)
    {
        var entity = await _context.Tournaments
            .Include(t => t.Matches)
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

    public async Task UpdateMatchResultAsync(Guid matchId, string state, string? winner)
    {
        var matchEntity = await _context.Matches.FindAsync(matchId);
        if (matchEntity != null)
        {
            matchEntity.State = state;
            matchEntity.Winner = winner;

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateNextMatchTeamsAsync(Guid matchId, string? teamA, string? teamB)
    {
        var nextMatch = await _context.Matches.FindAsync(matchId);
        if (nextMatch != null)
        {
            nextMatch.TeamA = teamA;
            nextMatch.TeamB = teamB;
        }

        await _context.SaveChangesAsync();
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