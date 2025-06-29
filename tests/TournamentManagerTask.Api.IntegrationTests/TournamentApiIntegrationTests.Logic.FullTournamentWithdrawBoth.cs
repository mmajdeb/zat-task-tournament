namespace TournamentManagerTask.Api.IntegrationTests;

public partial class TournamentApiIntegrationTests
{
    [Fact]
    public async Task FullTournament_WithdrawBothScenario_ShouldProduceCorrectWinner()
    {
        var tournament = await CreateTournamentAsync("WithdrawBoth Run Cup", 8);
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "FullTournamentResults_WithdrawBoth.txt");
        var matchResults = await ReadMatchResultsAsync(filePath);
        await FinishMatchesFromResultsAsync(tournament.Id, matchResults, skipFinished: true);
        var finalState = await GetTournamentStateAsync(tournament.Id);
        var finalRound = finalState.Matches.Max(m => m.Round);
        var finalMatch = finalState.Matches.First(m => m.Round == finalRound);
        Assert.Equal("Team 5", finalMatch.Winner);
        Assert.Equal("Finished", finalMatch.State);
    }
}
