using Claims.Application.Models;
using FluentValidation;

namespace Claims.Application.Validators;

public class PremiumComputeRequestModelValidator : BaseCoverPeriodValidator<PremiumComputeRequestModel>
{
    public PremiumComputeRequestModelValidator(): base()
    {
        RuleFor(x => x.Type).IsInEnum();
    }
}
