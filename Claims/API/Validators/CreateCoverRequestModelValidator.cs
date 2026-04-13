using Claims.Application.Extensions;
using Claims.Application.Models;
using FluentValidation;

namespace Claims.Application.Validators;

public class CreateCoverRequestModelValidator : AbstractValidator<CreateCoverRequestModel>
{
    public CreateCoverRequestModelValidator()
    {
        RuleFor(x => x.StartDate)
            .Must(CannotBeInPast)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x)
            .Must(BeSameYear)
            .WithMessage("Total insurance period cannot exceed 1 year.");
    }

    private bool BeSameYear(CreateCoverRequestModel model)
    {
        var endDate = model.EndDate.Kind == DateTimeKind.Local ? model.EndDate.ToUniversalTime() : model.EndDate;
        var startDate = model.StartDate.Kind == DateTimeKind.Local ? model.StartDate.ToUniversalTime() : model.StartDate;
        return (endDate - startDate).TotalDays <=365;
    }

    private bool CannotBeInPast(DateTime startDate)
    {
        if (startDate.Kind == DateTimeKind.Local)
        {
            startDate = startDate.ToUniversalTime();
        }
        return startDate >= DateTimeExtensions.UtcToday();
    }
}