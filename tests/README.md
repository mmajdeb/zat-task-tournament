# TournamentManagerTask Integration Tests

This folder contains integration tests for the `TournamentManagerTask.Api` project. The tests are implemented using [xUnit](https://xunit.net/) and target the API endpoints to ensure correct behavior of tournament creation, match progression, and edge cases.

## Structure

- **TournamentManagerTask.Api.IntegrationTests/**: Main integration test project.
  - `TournamentApiIntegrationTests.cs` and partials: Core test logic for API scenarios.
  - `IntegrationTestBase.cs`: Base class for integration tests, sets up the test server and HTTP client.
  - `TestData/`: Contains test data files for simulating full tournament runs and special cases.

## Test Scenarios

- **Tournament Creation**: Verifies tournaments can be created and brackets are generated correctly (including byes).
- **Match Progression**: Ensures that finishing matches advances winners and updates tournament state.
- **Edge Cases**: Handles scenarios like both teams withdrawing, and verifies correct winner calculation.
- **Full Tournament Runs**: Uses data-driven tests with files in `TestData/` to simulate entire tournaments.

## Usage

1. **Requirements**:

   - [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
   - macOS, Linux, or Windows

2. **Running Tests**:

   - From the repository root, run:
     ```bash
     dotnet test tests/TournamentManagerTask.Api.IntegrationTests
     ```

3. **Test Data**:
   - Test data files in `TestData/` are used for full tournament simulations.
   - **FullTournamentResults.txt**: Simulates a complete tournament run, specifying match results for each round. Used to verify correct winner calculation and bracket progression.
   - **FullTournamentResults_WithdrawBoth.txt**: Simulates a tournament scenario where both teams withdraw in a semi-final, testing edge case handling and correct winner determination when a slot is left empty.

## Dependencies

- `xunit`, `xunit.runner.visualstudio`: Test framework and runner
- `Microsoft.AspNetCore.Mvc.Testing`: For in-memory test server
- `coverlet.collector`: Code coverage

## Notes

- Tests are designed to run against the in-memory API, no external services required.
- See individual test files for scenario details and assertions.
