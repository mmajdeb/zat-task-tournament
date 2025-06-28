using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

/// <summary>
/// Controller for managing tournament operations including creation and retrieval of tournament state
/// </summary>
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

    /// <summary>
    /// Create a new tournament.
    /// </summary>
    /// <param name="request">The tournament creation request containing name and team count</param>
    /// <returns><see cref="CreateTournamentResponse"/></returns>
    /// <response code="201">Created. Returns the tournament ID.</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTournamentResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest request)
    {
        _logger.LogInformation("Creating tournament with name: {TournamentName}, teams count: {TeamsCount}",
            request.Name, request.TeamsCount);

        var id = await _tournamentService.CreateTournamentAsync(request.Name, request.TeamsCount);

        _logger.LogInformation("Tournament created successfully with ID: {TournamentId}", id);
        return CreatedAtAction(nameof(GetTournament), new { tournamentId = id }, new CreateTournamentResponse { Id = id });
    }

    /// <summary>
    /// Get tournament details by ID.
    /// </summary>
    /// <param name="tournamentId">The unique identifier of the tournament</param>
    /// <returns>Returns a <see cref="TournamentResponse"/> containing tournament details with all matches if found</returns>
    /// <response code="200">Tournament found and returned successfully.</response>
    /// <response code="404">Tournament not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{tournamentId}")]
    [ProducesResponseType(typeof(TournamentResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
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
                Winner = m.Winner,
                NextMatchId = m.NextMatchId
            }).ToList()
        };

        _logger.LogInformation("Successfully retrieved tournament {TournamentId} with {MatchCount} matches",
            tournamentId, response.Matches.Count);

        return Ok(response);
    }
}
