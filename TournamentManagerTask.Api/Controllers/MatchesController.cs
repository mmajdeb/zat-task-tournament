using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

/// <summary>
/// Controller for managing match operations including finishing matches and updating match results
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    private readonly ILogger<MatchesController> _logger; public MatchesController(IMatchService matchService, ILogger<MatchesController> logger)
    {
        _matchService = matchService;
        _logger = logger;
    }

    /// <summary>
    /// Finish a match.
    /// </summary>
    /// <param name="matchId">The unique identifier of the match to finish</param>
    /// <param name="request">The match finish request containing result type and winning team</param>
    /// <returns>Returns 204 No Content if the match was finished successfully</returns>
    /// <response code="204">Match finished successfully. No content returned.</response>
    /// <response code="400">Bad request data (e.g., invalid result type, missing winning team when required)</response>
    /// <response code="404">Match not found</response>
    /// <response code="409">Match cannot be finished (e.g., already finished, invalid state)</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Result types:
    /// - "Winner": One team wins, WinningTeam must specify the winning team
    /// - "WithdrawOne": One team withdraws, WinningTeam must specify the team that didn't withdraw
    /// - "WithdrawBoth": Both teams withdraw, WinningTeam should be null or empty
    /// </remarks>
    [HttpPost("{matchId}/finish")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> FinishMatch(Guid matchId, FinishMatchRequest request)
    {
        _logger.LogInformation("Finishing match {MatchId} with result: {Result}, winning team: {WinningTeam}",
            matchId, request.Result, request.WinningTeam);

        var result = new FinishResultDto
        {
            Result = request.Result,
            WinningTeam = request.WinningTeam
        };

        await _matchService.FinishMatchAsync(matchId, result);

        _logger.LogInformation("Match {MatchId} finished successfully", matchId);

        return NoContent();
    }
}
