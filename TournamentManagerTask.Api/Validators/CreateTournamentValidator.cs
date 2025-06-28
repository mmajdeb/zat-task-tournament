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

        RuleFor(x => x.TeamsCount)
            .GreaterThanOrEqualTo(2).WithMessage("At least 2 teams are required");
    }
}
