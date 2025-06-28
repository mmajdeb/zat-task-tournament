using TournamentManagerTask.Application.DTOs;

namespace TournamentManagerTask.Application.Interfaces;

public interface IMatchService
{
    Task FinishMatchAsync(Guid matchId, FinishResultDto input);
}
