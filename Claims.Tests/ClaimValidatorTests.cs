using Claims.Application.Validators;
using Claims.API.Validators;
using Claims.Domain.Entities;
using Claims.Application.Models;
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
        var claim = new Claim { Created = DateTime.Today };
        var result = validator.TestValidate(claim);
        result.ShouldHaveValidationErrorFor(x => x.Created)
            .WithErrorMessage("Created date must be within the period of the related Cover.");
    }

    [Fact]
    public void Should_Have_Error_When_Created_Date_Is_Outside_Cover_Period()
    {
        var cover = new Cover 
        { 
            StartDate = DateTime.Today.AddDays(1), 
            EndDate = DateTime.Today.AddDays(10) 
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = DateTime.Today };
        
        var result = validator.TestValidate(claim);
        result.ShouldHaveValidationErrorFor(x => x.Created)
            .WithErrorMessage("Created date must be within the period of the related Cover.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Created_Date_Is_Within_Cover_Period()
    {
        var cover = new Cover 
        { 
            StartDate = DateTime.Today.AddDays(-5), 
            EndDate = DateTime.Today.AddDays(5) 
        };
        var validator = new ClaimValidator(cover);
        var claim = new Claim { Created = DateTime.Today };
        
        var result = validator.TestValidate(claim);
        result.ShouldNotHaveValidationErrorFor(x => x.Created);
    }
}

