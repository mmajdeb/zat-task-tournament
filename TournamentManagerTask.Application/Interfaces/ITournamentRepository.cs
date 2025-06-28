using TournamentManagerTask.Domain.Entities;

namespace TournamentManagerTask.Application.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament> GetByIdAsync(Guid id);
    Task<Tournament> FindByMatchIdAsync(Guid matchId);
    Task SaveAsync(Tournament tournament);
}
