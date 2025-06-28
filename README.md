# TournamentManagerTask

TournamentManagerTask is a .NET solution for managing tournaments, matches, and related operations. It is organized into multiple projects following a clean architecture approach.

## Solution Structure

```
TournamentManagerTask/
├── TournamentManagerTask.sln
├── TournamentManagerTask.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Exceptions/
│   └── ...
├── TournamentManagerTask.Application/
│   ├── DTOs/
│   ├── Exceptions/
│   ├── Interfaces/
│   ├── Services/
│   └── ...
├── TournamentManagerTask.Infrastructure/
│   ├── Configurations/
│   ├── Data/
│   ├── Entities/
│   ├── Repositories/
│   └── ...
├── TournamentManagerTask.Api/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Extensions/
│   ├── Middlewares/
│   ├── Properties/
│   ├── Validators/
│   └── ...
└── README.md
```

## Projects Structure

- **TournamentManagerTask.Domain**: Domain entities, enums, and domain exceptions.
- **TournamentManagerTask.Application**: Application logic, DTOs, services, and interfaces.
- **TournamentManagerTask.Infrastructure**: Data access, repositories, and infrastructure dependencies.
- **TournamentManagerTask.Api**: ASP.NET Core Web API for exposing endpoints.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Clone the Project

Clone the repository to your local machine:

```bash
git clone https://github.com/mmajdeb/zat-task-tournament.git
cd TournamentManagerTask
```

### Build and Run

1. Restore dependencies:
   ```bash
   dotnet restore TournamentManagerTask.sln
   ```
2. Build the solution:
   ```bash
   dotnet build TournamentManagerTask.sln
   ```
3. Run the API project:
   ```bash
   dotnet run --project TournamentManagerTask.Api/TournamentManagerTask.Api.csproj
   ```

The API will be available at `https://localhost:5001` or `http://localhost:5000` by default.

## API Endpoints

### 1. Create Tournament

- **Endpoint:** `POST /api/tournaments`
- **Description:** Creates a new single-elimination tournament with the specified name and number of teams. The system automatically adds byes to reach the nearest power of 2 if needed.
- **Request Body:**
  ```json
  {
    "name": "World Championship 2024",
    "teamsCount": 8
  }
  ```
- **Response:**
  - `201 Created` with body:
    ```json
    {
      "id": "<tournament-guid>"
    }
    ```
  - `400 Bad Request` if input is invalid.
  - `500 Internal Server Error` on server error.

---

### 2. Get Tournament State

- **Endpoint:** `GET /api/tournaments/{tournamentId}`
- **Description:** Retrieves the current state of a tournament, including all matches, their states, and winners.
- **Response:**
  - `200 OK` with body:
    ```json
    {
      "name": "World Championship 2024",
      "matches": [
        {
          "matchId": "<match-guid>",
          "teamA": "Team Alpha",
          "teamB": "Team Beta",
          "state": "Pending",
          "winner": null,
          "round": 1,
          "nextMatchId": "<next-match-guid>"
        }
      ]
    }
    ```
  - `404 Not Found` if tournament does not exist.
  - `500 Internal Server Error` on server error.

---

### 3. Finish Match

- **Endpoint:** `POST /api/matches/{matchId}/finish`
- **Description:** Updates the result of a match. Advances the winner to the next round automatically.
- **Request Body:**
  ```json
  {
    "result": "Winner",
    "winningTeam": "Team Alpha"
  }
  ```
- **Response:**
  - `204 No Content` on success.
  - `400 Bad Request` if input is invalid.
  - `404 Not Found` if match does not exist.
  - `409 Conflict` if match cannot be finished (already finished, invalid state).
  - `500 Internal Server Error` on server error.

---

### API Documentation

- Interactive API documentation is available via Swagger UI at the root URL when running the API (e.g., [https://localhost:5001](https://localhost:5001)).

---
