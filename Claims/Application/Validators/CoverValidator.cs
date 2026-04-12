using FluentValidation;
using Claims.Domain.Entities;

namespace Claims.Application.Validators;

public class CoverValidator : AbstractValidator<Cover>
{
    public CoverValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
            .WithMessage("Total insurance period cannot exceed 1 year.");
    }
}
