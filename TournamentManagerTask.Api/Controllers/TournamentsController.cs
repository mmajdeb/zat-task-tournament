using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<TournamentsController> _logger;

    public TournamentsController(ITournamentService tournamentService, ILogger<TournamentsController> logger)
    {
        _tournamentService = tournamentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTournament(CreateTournamentRequest request)
    {
        _logger.LogInformation("Creating tournament with name: {TournamentName}, teams count: {TeamsCount}",
            request.Name, request.TeamsCount);

        var id = await _tournamentService.CreateTournamentAsync(request.Name, request.TeamsCount);

        _logger.LogInformation("Tournament created successfully with ID: {TournamentId}", id);
        return CreatedAtAction(nameof(GetTournament), new { tournamentId = id }, new { Id = id });
    }

    [HttpGet("{tournamentId}")]
    public async Task<ActionResult<TournamentResponse>> GetTournament(Guid tournamentId)
    {
        _logger.LogInformation("Retrieving tournament state for ID: {TournamentId}", tournamentId);
        var dto = await _tournamentService.GetTournamentStateAsync(tournamentId);

        _logger.LogDebug("Tournament found: {TournamentName} with {MatchCount} matches",
            dto.Name, dto.Matches.Count);

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

        _logger.LogInformation("Successfully retrieved tournament {TournamentId} with {MatchCount} matches",
            tournamentId, response.Matches.Count);

        return Ok(response);
    }
}
