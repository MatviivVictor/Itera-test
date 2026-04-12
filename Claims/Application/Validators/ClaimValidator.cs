using FluentValidation;
using Claims.Domain.Entities;
using Claims.Application.Interfaces;

namespace Claims.Application.Validators;

public class ClaimValidator : AbstractValidator<Claim>
{
    private readonly ICoversService _coversService;

    public ClaimValidator(ICoversService coversService)
    {
        _coversService = coversService;

        RuleFor(x => x.DamageCost)
            .LessThanOrEqualTo(100000)
            .WithMessage("Damage cost cannot exceed 100,000.");

        RuleFor(x => x)
            .MustAsync(async (claim, cancellationToken) =>
            {
                var cover = await _coversService.GetCoverAsync(claim.CoverId, cancellationToken);
                if (cover == null) return false;
                return claim.Created >= cover.StartDate && claim.Created <= cover.EndDate;
            })
            .WithMessage("Created date must be within the period of the related Cover.");
    }
}
