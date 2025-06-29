using System.Net.Http.Json;
using TournamentManagerTask.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace TournamentManagerTask.Api.IntegrationTests;

public partial class TournamentApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TournamentApiIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    private async Task<CreateTournamentResponse> CreateTournamentAsync(string name, int teamsCount)
    {
        var request = new CreateTournamentRequest { Name = name, TeamsCount = teamsCount };
        var response = await _client.PostAsJsonAsync("/api/tournaments", request);
        response.EnsureSuccessStatusCode();
        var tournament = await response.Content.ReadFromJsonAsync<CreateTournamentResponse>();
        Assert.NotNull(tournament);
        return tournament;
    }

    private async Task<TournamentResponse> GetTournamentStateAsync(Guid tournamentId)
    {
        var getResponse = await _client.GetAsync($"/api/tournaments/{tournamentId}");
        getResponse.EnsureSuccessStatusCode();
        var state = await getResponse.Content.ReadFromJsonAsync<TournamentResponse>();
        Assert.NotNull(state);
        return state;
    }

    private async Task FinishMatchAsync(Guid matchId, FinishMatchRequest finishRequest)
    {
        var finishResponse = await _client.PostAsJsonAsync($"/api/matches/{matchId}/finish", finishRequest);
        finishResponse.EnsureSuccessStatusCode();
    }

    private async Task<List<dynamic>> ReadMatchResultsAsync(string filePath)
    {
        Assert.True(File.Exists(filePath), $"Test data file not found: {filePath}");
        var lines = await File.ReadAllLinesAsync(filePath);
        return lines
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.Trim().StartsWith("#"))
            .Select(l => l.Split(';'))
            .Select(parts => new
            {
                Round = int.Parse(parts[0]),
                TeamA = parts[1].Split(',')[0].Trim(),
                TeamB = parts[1].Split(',')[1].Trim(),
                Result = parts[2],
                WinnerId = parts.Length > 3 ? parts[3] : null
            })
            .Cast<dynamic>()
            .ToList();
    }

    private async Task FinishMatchesFromResultsAsync(Guid tournamentId, IEnumerable<dynamic> matchResults, bool skipFinished = false)
    {
        foreach (var result in matchResults)
        {
            var state = await GetTournamentStateAsync(tournamentId);
            var match = state.Matches
                .FirstOrDefault(m => m.Round == result.Round &&
                    ((string.Equals(m.TeamA, result.TeamA, StringComparison.OrdinalIgnoreCase) || result.TeamA == "null") &&
                     (string.Equals(m.TeamB, result.TeamB, StringComparison.OrdinalIgnoreCase) || result.TeamB == "null") ||
                     (string.Equals(m.TeamA, result.TeamB, StringComparison.OrdinalIgnoreCase) || result.TeamB == "null") &&
                     (string.Equals(m.TeamB, result.TeamA, StringComparison.OrdinalIgnoreCase) || result.TeamA == "null")));
            Assert.NotNull(match);
            if (skipFinished && match.State == "Finished")
            {
                _output?.WriteLine($"Match {match.MatchId} is already finished, skipping.");
                continue;
            }
            var finishRequest = new FinishMatchRequest { Result = result.Result };
            if ((result.Result == "Winner" || result.Result == "WithdrawOne") && !string.IsNullOrEmpty(result.WinnerId))
                finishRequest.WinningTeamId = result.WinnerId;
            await FinishMatchAsync(match.MatchId, finishRequest);
        }
    }
}
