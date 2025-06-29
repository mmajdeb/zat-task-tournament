using System.Net;
using System.Net.Http.Json;
using TournamentManagerTask.Api.DTOs;

namespace TournamentManagerTask.Api.IntegrationTests;

public partial class TournamentApiIntegrationTests
{
    [Fact]
    public async Task CreateTournament_ShouldReturnBracketWithByes()
    {
        var tournament = await CreateTournamentAsync("Summer Cup", 14);
        Assert.NotEqual(Guid.Empty, tournament.Id);
    }

    [Fact]
    public async Task GetTournamentState_ShouldReturnAllMatches()
    {
        var tournament = await CreateTournamentAsync("Test Cup", 8);
        var state = await GetTournamentStateAsync(tournament.Id);
        Assert.Equal("Test Cup", state.Name);
        Assert.NotEmpty(state.Matches);
    }

    [Fact]
    public async Task FinishMatch_ShouldAdvanceWinnerToNextRound()
    {
        var tournament = await CreateTournamentAsync("Advance Cup", 4);
        var state = await GetTournamentStateAsync(tournament.Id);
        var firstMatch = state.Matches[0];
        var winnerId = firstMatch.TeamA ?? firstMatch.TeamB;
        var finishRequest = new FinishMatchRequest { Result = "Winner", WinningTeamId = winnerId };
        await FinishMatchAsync(firstMatch.MatchId, finishRequest);
        var updatedState = await GetTournamentStateAsync(tournament.Id);
        Assert.Contains(updatedState.Matches, m => m.Round == 2 && (m.TeamA == winnerId || m.TeamB == winnerId));
    }

    [Fact]
    public async Task FinishMatch_BothWithdraw_ShouldLeaveNextSlotEmpty()
    {
        var tournament = await CreateTournamentAsync("Withdraw Cup", 4);
        var state = await GetTournamentStateAsync(tournament.Id);
        var firstMatch = state.Matches[0];
        var finishRequest = new FinishMatchRequest { Result = "WithdrawBoth" };
        await FinishMatchAsync(firstMatch.MatchId, finishRequest);
        var updatedState = await GetTournamentStateAsync(tournament.Id);
        var nextRoundMatch = updatedState.Matches.Find(m => m.Round == 2);
        Assert.NotNull(nextRoundMatch);
        Assert.True(nextRoundMatch.TeamA == null || nextRoundMatch.TeamB == null);
    }

    [Fact]
    public async Task CannotFinishMatchTwice()
    {
        var tournament = await CreateTournamentAsync("Double Finish Cup", 4);
        var state = await GetTournamentStateAsync(tournament.Id);
        var firstMatch = state.Matches[0];
        var winnerId = firstMatch.TeamA ?? firstMatch.TeamB;
        var finishRequest = new FinishMatchRequest { Result = "Winner", WinningTeamId = winnerId };
        await FinishMatchAsync(firstMatch.MatchId, finishRequest);
        var secondFinishResponse = await _client.PostAsJsonAsync($"/api/matches/{firstMatch.MatchId}/finish", finishRequest);
        Assert.Equal(HttpStatusCode.Conflict, secondFinishResponse.StatusCode);
    }
}
