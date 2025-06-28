using System;
using System.Linq;
using FluentValidation;
using TournamentManagerTask.Api.DTOs;

namespace TournamentManagerTask.Api.Validators;

public class FinishMatchValidator : AbstractValidator<FinishMatchRequest>
{
    private static readonly string[] ValidResults = { "Winner", "WithdrawOne", "WithdrawBoth" };

    public FinishMatchValidator()
    {
        RuleFor(x => x.Result)
            .NotEmpty().WithMessage("Result is required")
            .Must(result => ValidResults.Contains(result))
            .WithMessage("Invalid result type. Must be Winner, WithdrawOne, or WithdrawBoth.");

        When(x => x.Result == "Winner" || x.Result == "WithdrawOne", () =>
        {
            RuleFor(x => x.WinningTeam)
                .NotEmpty().WithMessage("WinningTeam is required for result types Winner or WithdrawOne");
        });

        When(x => x.Result == "WithdrawBoth", () =>
        {
            RuleFor(x => x.WinningTeam)
                .Empty().WithMessage("WinningTeam must be empty when both teams withdraw");
        });
    }
}
