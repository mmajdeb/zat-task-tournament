using System.Collections.Generic;

namespace TournamentManagerTask.Api.DTOs;

public class CreateTournamentRequest
{
    public string Name { get; set; } = string.Empty;
    public List<string> TeamNames { get; set; } = new();
}
