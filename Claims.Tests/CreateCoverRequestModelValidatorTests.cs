using Claims.Application.Extensions;
using Claims.Application.Validators;
using Claims.Application.Models;
using FluentValidation.TestHelper;
using Xunit;

namespace Claims.Tests;

public class CreateCoverRequestModelValidatorTests
{
    private readonly CreateCoverRequestModelValidator _validator;

    public CreateCoverRequestModelValidatorTests()
    {
        _validator = new CreateCoverRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_In_Past()
    {
        var model = new CreateCoverRequestModel { StartDate = DateTimeExtensions.UtcToday().AddDays(-1) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date cannot be in the past.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_StartDate_Is_Today()
    {
        var model = new CreateCoverRequestModel { StartDate = DateTimeExtensions.UtcToday(), EndDate = DateTimeExtensions.UtcToday().AddDays(10) };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_Have_Error_When_Insurance_Period_Exceeds_1_Year()
    {
        var model = new CreateCoverRequestModel 
        { 
            StartDate = DateTimeExtensions.UtcToday(), 
            EndDate = DateTimeExtensions.UtcToday().AddDays(367) 
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Total insurance period cannot exceed 1 year.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Insurance_Period_Is_Exactly_1_Year()
    {
        var model = new CreateCoverRequestModel 
        { 
            StartDate = DateTimeExtensions.UtcToday(), 
            EndDate = DateTimeExtensions.UtcToday().AddDays(365) 
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
