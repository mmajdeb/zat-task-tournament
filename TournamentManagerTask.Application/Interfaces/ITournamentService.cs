using TournamentManagerTask.Application.DTOs;

namespace TournamentManagerTask.Application.Interfaces;

public interface ITournamentService
{
    Task<Guid> CreateTournamentAsync(string name, List<string> teamNames);
    Task<TournamentDto> GetTournamentStateAsync(Guid tournamentId);
}
