using System;

namespace TournamentManagerTask.Api.DTOs;

/// <summary>
/// Response model for creating a new tournament
/// </summary>
public class CreateTournamentResponse
{
    /// <summary>
    /// The unique identifier of the created tournament
    /// </summary>
    /// <example>e7b1f8c2-3d4e-4f5a-8b6c-9d0e1f2a3b4c</example>
    public Guid Id { get; set; }
}