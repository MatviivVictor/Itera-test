using Claims.Domain.Entities;
using FluentValidation;

namespace Claims.Application.Validators;

public class ClaimValidator : AbstractValidator<Claim>
{
    public ClaimValidator(Cover? cover = null)
    {
        RuleFor(x => x.Created)
            .Must(x => cover is not null).WithMessage("Cover does not exist.")
            .Must(x => BeWithinCoverPeriod(x, cover))
            .WithMessage("Created date must be within the period of the related Cover.");
    }
    
    private bool BeWithinCoverPeriod(DateTime created, Cover? cover)
    {
        if (cover is null) return false;
    
        return created >= cover.StartDate && created <= cover.EndDate;
    }
}