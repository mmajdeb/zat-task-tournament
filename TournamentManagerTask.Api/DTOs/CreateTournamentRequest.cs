using System.Collections.Generic;

namespace TournamentManagerTask.Api.DTOs;

/// <summary>
/// Request model for creating a new tournament
/// </summary>
public class CreateTournamentRequest
{
    /// <summary>
    /// The name of the tournament
    /// </summary>
    /// <example>Saudi Pro League Championship</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The number of teams participating in the tournament. Must be a power of 2 (4, 8, 16, etc.)
    /// </summary>
    /// <example>8</example>
    public int TeamsCount { get; set; }
}
