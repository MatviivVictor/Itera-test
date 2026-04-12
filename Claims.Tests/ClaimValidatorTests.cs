using Claims.Application.Validators;
using Claims.Domain.Entities;
using Claims.Application.Interfaces;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace Claims.Tests;

public class ClaimValidatorTests
{
    private readonly ClaimValidator _validator;
    private readonly Mock<ICoversService> _coversServiceMock;

    public ClaimValidatorTests()
    {
        _coversServiceMock = new Mock<ICoversService>();
        _validator = new ClaimValidator(_coversServiceMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_DamageCost_Exceeds_100000()
    {
        var claim = new Claim { DamageCost = 100001 };
        var result = await _validator.TestValidateAsync(claim);
        result.ShouldHaveValidationErrorFor(x => x.DamageCost)
            .WithErrorMessage("Damage cost cannot exceed 100,000.");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_DamageCost_Is_100000()
    {
        var claim = new Claim { DamageCost = 100000 };
        // We need to setup cover for the other rule not to fail if it's executed
        var cover = new Cover 
        { 
            Id = "cover-1", 
            StartDate = DateTime.Today.AddDays(-1), 
            EndDate = DateTime.Today.AddDays(1) 
        };
        claim.CoverId = cover.Id;
        claim.Created = DateTime.Today;
        
        _coversServiceMock.Setup(x => x.GetCoverAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        var result = await _validator.TestValidateAsync(claim);
        result.ShouldNotHaveValidationErrorFor(x => x.DamageCost);
    }

    [Fact]
    public async Task Should_Have_Error_When_Cover_Does_Not_Exist()
    {
        var claim = new Claim { CoverId = "non-existent", Created = DateTime.Today };
        _coversServiceMock.Setup(x => x.GetCoverAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cover?)null);

        var result = await _validator.TestValidateAsync(claim);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Created date must be within the period of the related Cover.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Created_Date_Is_Outside_Cover_Period()
    {
        var cover = new Cover 
        { 
            Id = "cover-1", 
            StartDate = DateTime.Today.AddDays(1), 
            EndDate = DateTime.Today.AddDays(10) 
        };
        var claim = new Claim { CoverId = cover.Id, Created = DateTime.Today };
        
        _coversServiceMock.Setup(x => x.GetCoverAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        var result = await _validator.TestValidateAsync(claim);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Created date must be within the period of the related Cover.");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Created_Date_Is_Within_Cover_Period()
    {
        var cover = new Cover 
        { 
            Id = "cover-1", 
            StartDate = DateTime.Today.AddDays(-5), 
            EndDate = DateTime.Today.AddDays(5) 
        };
        var claim = new Claim { CoverId = cover.Id, Created = DateTime.Today, DamageCost = 500 };
        
        _coversServiceMock.Setup(x => x.GetCoverAsync(cover.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        var result = await _validator.TestValidateAsync(claim);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
