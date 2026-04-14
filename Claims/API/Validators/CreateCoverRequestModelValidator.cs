using Claims.Application.Models;
using FluentValidation;

namespace Claims.Application.Validators;

public class CreateCoverRequestModelValidator : BaseCoverPeriodValidator<CreateCoverRequestModel>
{
    public CreateCoverRequestModelValidator():base()
    {
        RuleFor(x => x.Type).IsInEnum();
    }
}