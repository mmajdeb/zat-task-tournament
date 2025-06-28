using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TournamentManagerTask.Api.DTOs;
using TournamentManagerTask.Application.DTOs;
using TournamentManagerTask.Application.Interfaces;

namespace TournamentManagerTask.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpPost("{matchId}/finish")]
    public async Task<IActionResult> FinishMatch(Guid matchId, FinishMatchRequest request)
    {
        var result = new FinishResultDto
        {
            Result = request.Result,
            WinningTeam = request.WinningTeam
        };

        await _matchService.FinishMatchAsync(matchId, result);
        return NoContent();
    }
}
