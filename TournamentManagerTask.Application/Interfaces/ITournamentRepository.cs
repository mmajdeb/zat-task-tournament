using TournamentManagerTask.Domain.Entities;
using TournamentManagerTask.Domain.Enums;

namespace TournamentManagerTask.Application.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<Tournament?> FindByMatchIdAsync(Guid matchId);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task UpdateMatchResultAsync(Guid matchId, string state, string? winner);
    Task UpdateNextMatchAsync(Guid matchId, string? teamA, string? teamB, string? winner, MatchState state);
}
