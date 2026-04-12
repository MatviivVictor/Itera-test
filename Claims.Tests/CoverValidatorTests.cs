using Claims.Application.Validators;
using Claims.Domain.Entities;
using FluentValidation.TestHelper;
using Xunit;

namespace Claims.Tests;

public class CoverValidatorTests
{
    private readonly CoverValidator _validator;

    public CoverValidatorTests()
    {
        _validator = new CoverValidator();
    }

    [Fact]
    public void Should_Have_Error_When_StartDate_Is_In_Past()
    {
        var cover = new Cover { StartDate = DateTime.Today.AddDays(-1) };
        var result = _validator.TestValidate(cover);
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date cannot be in the past.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_StartDate_Is_Today()
    {
        var cover = new Cover { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(10) };
        var result = _validator.TestValidate(cover);
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Should_Have_Error_When_Insurance_Period_Exceeds_1_Year()
    {
        var cover = new Cover 
        { 
            StartDate = DateTime.Today, 
            EndDate = DateTime.Today.AddDays(367) 
        };
        var result = _validator.TestValidate(cover);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Total insurance period cannot exceed 1 year.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Insurance_Period_Is_Exactly_1_Year()
    {
        var cover = new Cover 
        { 
            StartDate = DateTime.Today, 
            EndDate = DateTime.Today.AddDays(365) 
        };
        var result = _validator.TestValidate(cover);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
