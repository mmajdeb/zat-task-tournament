using TournamentManagerTask.Domain.Entities;

namespace TournamentManagerTask.Application.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<Tournament?> FindByMatchIdAsync(Guid matchId);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task UpdateMatchResultAsync(Guid matchId, string state, string? winner);
    Task UpdateNextMatchTeamsAsync(Guid matchId, string? teamA, string? teamB);
}
