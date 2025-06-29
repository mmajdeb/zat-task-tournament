namespace TournamentManagerTask.Api.DTOs;

/// <summary>
/// Request model for finishing a match with a result
/// </summary>
public class FinishMatchRequest
{
    /// <summary>
    /// The result type of the match. Valid values: "Winner", "WithdrawOne", "WithdrawBoth"
    /// </summary>
    /// <example>Winner</example>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// The winning team ID. Required when Result is "Winner" or "WithdrawOne". Should be null for "WithdrawBoth"
    /// </summary>
    /// <example>Team 2</example>
    public string WinningTeamId { get; set; }
}
