## C# Web API Interview Task

### **Objective**

Create a C# Web API application that manages **single-elimination tournaments** (“cup format”) with the following features:

---

### **Functional Requirements**

1. **Create Tournament**

   - **Endpoint:** `POST /tournaments`
   - **Input:**

     ```json
     {
       "name": "Summer Cup",
       "teamsCount": 14
     }
     ```

   - The API:

     - Creates a single-elimination bracket.
     - Automatically adds **byes** to reach the nearest power of 2 (e.g., 16).
     - Persists the tournament and matches (in memory or in a simple database).

   - Each **match** has:

     - A unique ID
     - References to the two competing teams (or null if bye)
     - A round number
     - A state (`Pending`, `Finished`)
     - A winner (if finished)

2. **Get Tournament State**

   - **Endpoint:** `GET /tournaments/{tournamentId}`
   - Returns:

     - Tournament name
     - List of all matches with:

       - Match ID
       - Competing teams
       - Match state
       - Winner (if any)
       - Round number

3. **Update Match State**

   - **Endpoint:** `POST /matches/{matchId}/finish`
   - **Input:**

     ```json
     {
       "result": "Winner",
       "winningTeamId": "team-2"
     }
     ```

     - `result` can be:

       - `"Winner"` (one team won)
       - `"WithdrawOne"` (one team withdrew)
       - `"WithdrawBoth"` (both teams withdrew)

   - When a match is finished:

     - **The tournament state must be updated automatically**:

       - If there is a winner (or a single team advancing because the other withdraw), that team **must be assigned to the next round’s corresponding match**.
       - If both teams withdrew, **no team advances**, and the next round slot remains empty with the other team should win automatically.

     - Prevent finishing the same match twice.

   - Example:

     - If Match A in Round 1 is finished with a winner, the winning team is automatically assigned as **Team 1** (or Team 2) in the next round’s match that depends on Match A.

---

### **Implementation Requirements**

- Use **C# .NET (ASP.NET Core)** Web API.
- Keep the architecture clean (organize services, controllers, models).
- You may store data **in memory** (singleton service) or use a simple persistence (SQLite).
- Implement validation.
- Provide clear instructions to run the API.

---

### **Deliverables**

- A working C# Web API project.
- Source code (GitHub).
- Instructions on how to run the project.

---

### **Evaluation Criteria**

- Code correctness and clarity.
- Accurate handling of byes and withdrawals.
- Automatic progression of tournament rounds when matches are finished.
- Clean separation of concerns.
