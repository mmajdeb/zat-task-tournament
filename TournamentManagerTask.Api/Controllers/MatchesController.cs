using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

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

    [HttpPost("{matchId}/finish")]
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
