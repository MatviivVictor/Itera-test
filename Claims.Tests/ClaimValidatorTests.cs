using Claims.Application.Extensions;
using Claims.Application.Validators;
using Claims.Domain.Entities;
using FluentValidation.TestHelper;
using Xunit;

namespace Claims.Tests;

public class ClaimValidatorTests
{
    private readonly ClaimValidator _validator;

    public ClaimValidatorTests()
    {
    }

    [Fact]
    public void Should_Have_Error_When_Cover_Is_Null()
    {
        var validator = new ClaimValidator(null);
        var claim = new Claim { Created = DateTimeExtensions.UtcToday() };
        var result = validator.TestValidate(claim);
        result.ShouldHaveValidationErrorFor(x => x.Created)
            .WithErrorMessage("Cover does not exist.");
    }

    [Fact]
    public void Should_Have_Error_When_Created_Date_Is_Outside_Cover_Period()
    {
        var cover = new Cover 
        { 
            StartDate = DateTimeExtensions.UtcToday().AddDays(1), 
            EndDate = DateTimeExtensions.UtcToday().AddDays(10) 
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = DateTimeExtensions.UtcToday() };
        
        var result = validator.TestValidate(claim);
        result.ShouldHaveValidationErrorFor(x => x.Created)
            .WithErrorMessage("Created date must be within the period of the related Cover.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Created_Date_Is_Within_Cover_Period()
    {
        var cover = new Cover 
        { 
            StartDate = DateTimeExtensions.UtcToday().AddDays(-5), 
            EndDate = DateTimeExtensions.UtcToday().AddDays(5) 
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = DateTimeExtensions.UtcToday() };
        
        var result = validator.TestValidate(claim);
        result.ShouldNotHaveValidationErrorFor(x => x.Created);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Created_Date_Is_On_Start_Date()
    {
        var startDate = DateTimeExtensions.UtcToday();
        var cover = new Cover
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(10)
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = startDate };

        var result = validator.TestValidate(claim);
        result.ShouldNotHaveValidationErrorFor(x => x.Created);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Created_Date_Is_On_End_Date()
    {
        var endDate = DateTimeExtensions.UtcToday().AddDays(10);
        var cover = new Cover
        {
            StartDate = DateTimeExtensions.UtcToday(),
            EndDate = endDate
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = endDate };

        var result = validator.TestValidate(claim);
        result.ShouldNotHaveValidationErrorFor(x => x.Created);
    }
}

