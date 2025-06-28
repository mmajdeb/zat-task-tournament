using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTournament(CreateTournamentRequest request)
    {
        var id = await _tournamentService.CreateTournamentAsync(request.Name, request.TeamNames);
        return CreatedAtAction(nameof(GetTournament), new { tournamentId = id }, new { Id = id });
    }

    [HttpGet("{tournamentId}")]
    public async Task<ActionResult<TournamentResponse>> GetTournament(Guid tournamentId)
    {
        var dto = await _tournamentService.GetTournamentStateAsync(tournamentId);

        var response = new TournamentResponse
        {
            Name = dto.Name,
            Matches = dto.Matches.Select(m => new MatchResponse
            {
                MatchId = m.Id,
                Round = m.Round,
                State = m.State,
                TeamA = m.TeamA,
                TeamB = m.TeamB,
                Winner = m.Winner
            }).ToList()
        };

        return Ok(response);
    }
}
