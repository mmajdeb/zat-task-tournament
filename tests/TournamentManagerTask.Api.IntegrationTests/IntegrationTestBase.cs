using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using TournamentManagerTask.Api;
using Xunit;

namespace TournamentManagerTask.Api.IntegrationTests;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}
