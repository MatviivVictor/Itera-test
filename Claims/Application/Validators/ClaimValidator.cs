using FluentValidation;
using Claims.Domain.Entities;

namespace Claims.Application.Validators;

public class ClaimValidator : AbstractValidator<Claim>
{
    public ClaimValidator()
    {
        RuleFor(x => x.CoverId)
            .NotEmpty()
            .WithMessage("CoverId is required.");

        RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid claim type.");

        RuleFor(x => x.DamageCost)
            .LessThanOrEqualTo(100000)
            .WithMessage("Damage cost cannot exceed 100,000.");
    }
}

public class ClaimCreatedValidation : AbstractValidator<Claim>
{
    public ClaimCreatedValidation(Cover? cover = null)
    {
        var cover1 = cover;
        
        RuleFor(x => x.Created)
            .Must(x => BeWithinCoverPeriod(x, cover1))
            .WithMessage("Created date must be within the period of the related Cover.");
    }
    
    private bool BeWithinCoverPeriod(DateTime created, Cover? cover)
    {
        if (cover is null) return false;
        
        var createdUtc = created.Kind == DateTimeKind.Utc
            ? created
            : created.ToUniversalTime();
    
        var startUtc = cover.StartDate.Kind == DateTimeKind.Utc
            ? cover.StartDate
            : cover.StartDate.ToUniversalTime();
    
        var endUtc = cover.EndDate.Kind == DateTimeKind.Utc
            ? cover.EndDate
            : cover.EndDate.ToUniversalTime();
    
        return createdUtc >= startUtc && createdUtc <= endUtc;
    }
}
