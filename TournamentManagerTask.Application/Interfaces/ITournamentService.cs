using TournamentManagerTask.Application.DTOs;

namespace TournamentManagerTask.Application.Interfaces;

public interface ITournamentService
{
    Task<Guid> CreateTournamentAsync(string name, int teamsCount);
    Task<TournamentDto> GetTournamentStateAsync(Guid tournamentId);
}
