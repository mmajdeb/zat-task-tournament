using System.Linq;
using FluentValidation;
using TournamentManagerTask.Api.DTOs;

namespace TournamentManagerTask.Api.Validators;

public class CreateTournamentValidator : AbstractValidator<CreateTournamentRequest>
{
    public CreateTournamentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tournament name is required");

        RuleFor(x => x.TeamNames)
            .NotNull().WithMessage("Teams list must not be null")
            .Must(x => x.Count >= 2).WithMessage("At least 2 teams are required")
            .Must(x => x.All(name => !string.IsNullOrWhiteSpace(name)))
            .WithMessage("Team names cannot be empty or whitespace");
    }
}
