using Claims.Application.Factories;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Xunit;

namespace Claims.Tests;

public class PremiumServiceTests
{
    private readonly PremiumService _premiumService;

    public PremiumServiceTests()
    {
        _premiumService = new PremiumService(MultiplierStrategyFactory.GetStrategy);
    }

    [Theory]
    [InlineData(25, CoverType.Yacht, 34375.0)]
    [InlineData(125, CoverType.Yacht, 165343.75)]
    [InlineData(225, CoverType.Yacht, 294112.5)]
    [InlineData(25, CoverType.PassengerShip, 37500.0)]
    [InlineData(125, CoverType.PassengerShip, 184650.0)]
    [InlineData(225, CoverType.PassengerShip, 330975.0)]
    [InlineData(25, CoverType.ContainerShip, 40625.0)]
    [InlineData(125, CoverType.ContainerShip, 200037.5)]
    [InlineData(225, CoverType.ContainerShip, 358556.25)]
    [InlineData(25, CoverType.BulkCarrier, 40625.0)]
    [InlineData(125, CoverType.BulkCarrier, 200037.5)]
    [InlineData(225, CoverType.BulkCarrier, 358556.25)]
    [InlineData(25, CoverType.Tanker, 46875.0)]
    [InlineData(125, CoverType.Tanker, 230812.5)]
    [InlineData(225, CoverType.Tanker, 413718.75)]
    public void ComputePremium_ShouldReturnExpectedValue(int days, CoverType coverType, decimal expectedPremium)
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = startDate.AddDays(days);

        // Act
        var actualPremium = _premiumService.ComputePremium(startDate, endDate, coverType);

        // Assert
        Assert.Equal(expectedPremium, actualPremium);
    }

    [Fact]
    public void ComputePremium_WithRequestModel_ShouldReturnExpectedValue()
    {
        // Arrange
        var request = new Claims.Application.Models.PremiumComputeRequestModel
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 1).AddDays(25),
            Type = CoverType.Yacht
        };
        var expectedPremium = 34375.0m;

        // Act
        var actualPremium = _premiumService.ComputePremium(request);

        // Assert
        Assert.Equal(expectedPremium, actualPremium);
    }
}
