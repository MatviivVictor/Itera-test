using Claims.Application.Models;
using FluentValidation;

namespace Claims.API.Validators;

public class CreateClaimRequestModelValidator : AbstractValidator<CreateClaimRequestModel>
{
    public CreateClaimRequestModelValidator()
    {
        RuleFor(x => x.CoverId)
            .NotEmpty()
            .WithMessage("CoverId is required.");

        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid claim type.");

        RuleFor(x => x.DamageCost)
            .GreaterThanOrEqualTo(0).WithMessage("Damage cost cannot be negative.")
            .LessThanOrEqualTo(100000)
            .WithMessage("Damage cost cannot exceed 100,000.");
    }
}