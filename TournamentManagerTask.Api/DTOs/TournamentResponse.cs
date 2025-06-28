using System;
using System.Collections.Generic;

namespace TournamentManagerTask.Api.DTOs;

/// <summary>
/// Response model containing tournament information and all its matches
/// </summary>
public class TournamentResponse
{
    /// <summary>
    /// The name of the tournament
    /// </summary>
    /// <example>Saudi Pro League Championship</example>
    public string Name { get; set; }

    /// <summary>
    /// List of all matches in the tournament
    /// </summary>
    public List<MatchResponse> Matches { get; set; }
}

/// <summary>
/// Response model for a single match within a tournament
/// </summary>
public class MatchResponse
{
    /// <summary>
    /// Unique identifier for the match
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid MatchId { get; set; }

    /// <summary>
    /// Name of the first team
    /// </summary>
    /// <example>Team Alpha</example>
    public string TeamA { get; set; }

    /// <summary>
    /// Name of the second team
    /// </summary>
    /// <example>Team Beta</example>
    public string TeamB { get; set; }

    /// <summary>
    /// Current state of the match. Valid values: "Pending", "Finished"
    /// </summary>
    /// <example>Pending</example>
    public string State { get; set; }

    /// <summary>
    /// Name of the winning team (only set when match is finished)
    /// </summary>
    /// <example>Team Alpha</example>
    public string Winner { get; set; }

    /// <summary>
    /// Tournament round number (1 = first round, 2 = second round, etc.)
    /// </summary>
    /// <example>1</example>
    public int Round { get; set; }

    /// <summary>
    /// ID of the next match that the winner will advance to (null for final match)
    /// </summary>
    /// <example>660e8400-e29b-41d4-a716-446655440000</example>
    public Guid? NextMatchId { get; set; }
}
