namespace TournamentManagerTask.Api.IntegrationTests;

public partial class TournamentApiIntegrationTests
{
    [Fact]
    public async Task FullTournament_AllRoundsAndMatches_ShouldProduceCorrectWinner()
    {
        var tournament = await CreateTournamentAsync("Full Run Cup", 8);
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "FullTournamentResults.txt");
        var matchResults = await ReadMatchResultsAsync(filePath);
        await FinishMatchesFromResultsAsync(tournament.Id, matchResults);
        var finalState = await GetTournamentStateAsync(tournament.Id);
        var finalRound = finalState.Matches.Max(m => m.Round);
        var finalMatch = finalState.Matches.First(m => m.Round == finalRound);
        Assert.Equal("Team 1", finalMatch.Winner);
        Assert.Equal("Finished", finalMatch.State);
    }
}
