using System;
using System.Collections.Generic;

namespace TournamentManagerTask.Api.DTOs;

public class TournamentResponse
{
    public string Name { get; set; }
    public List<MatchResponse> Matches { get; set; }
}

public class MatchResponse
{
    public Guid MatchId { get; set; }
    public string? TeamA { get; set; }
    public string? TeamB { get; set; }
    public string State { get; set; }
    public string? Winner { get; set; }
    public int Round { get; set; }
}
