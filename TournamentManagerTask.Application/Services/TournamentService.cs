using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Exceptions;
using TournamentManagerTask.Application.Interfaces;
using TournamentManagerTask.Domain.Entities;

namespace TournamentManagerTask.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _repository;

    public TournamentService(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateTournamentAsync(string name, int teamsCount)
    {
        var tournament = Tournament.Create(name, teamsCount);
        await _repository.CreateAsync(tournament);
        return tournament.Id;
    }

    public async Task<TournamentDto> GetTournamentStateAsync(Guid tournamentId)
    {
        var tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new TournamentNotFoundException(tournamentId);

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
