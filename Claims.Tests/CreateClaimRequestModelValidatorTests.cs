using Claims.API.Validators;
using Claims.Application.Models;
using Claims.Domain.Entities;
using FluentValidation.TestHelper;
using Xunit;

namespace Claims.Tests;

public class CreateClaimRequestModelValidatorTests
{
    private readonly CreateClaimRequestModelValidator _validator;

    public CreateClaimRequestModelValidatorTests()
    {
        _validator = new CreateClaimRequestModelValidator();
    }

    [Fact]
    public void Should_Have_Error_When_CoverId_Is_Empty()
    {
        var model = new CreateClaimRequestModel { CoverId = string.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CoverId)
            .WithErrorMessage("CoverId is required.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_CoverId_Is_Provided()
    {
        var model = new CreateClaimRequestModel { CoverId = "test-id" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.CoverId);
    }

    [Fact]
    public void Should_Have_Error_When_Type_Is_Invalid()
    {
        var model = new CreateClaimRequestModel { Type = (ClaimType)999 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Invalid claim type.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Type_Is_Valid()
    {
        var model = new CreateClaimRequestModel { Type = ClaimType.Fire };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_Have_Error_When_DamageCost_Exceeds_100000()
    {
        var model = new CreateClaimRequestModel { DamageCost = 100001 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DamageCost)
            .WithErrorMessage("Damage cost cannot exceed 100,000.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_DamageCost_Is_100000()
    {
        var model = new CreateClaimRequestModel { DamageCost = 100000 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DamageCost);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DamageCost_Is_Less_Than_100000()
    {
        var model = new CreateClaimRequestModel { DamageCost = 50000 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DamageCost);
    }
}
