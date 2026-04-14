using Claims.Application.Extensions;
using Claims.Application.Interfaces;
using Claims.Application.Models;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using Moq;
using Xunit;

namespace Claims.Tests;

public class CoversServiceTests
{
    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly Mock<IAuditer> _auditerMock;
    private readonly Mock<IComputePremiumMultiplierStategy> _strategyMock;
    private readonly CoversService _coversService;

    public CoversServiceTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _auditerMock = new Mock<IAuditer>();
        _strategyMock = new Mock<IComputePremiumMultiplierStategy>();

        _coversService = new CoversService(
            _coversRepositoryMock.Object,
            _auditerMock.Object,
            (coverType) => _strategyMock.Object);
    }

    [Theory]
    [InlineData(CoverType.Yacht, 1.1, 1, 3946.25)] // 1375 + 1306.25 + 1265 = 3946.25
    [InlineData(CoverType.PassengerShip, 1.2, 1, 4425.0)] // 1500 + 1470 + 1455 = 4425
    [InlineData(CoverType.Tanker, 1.5, 1, 5531.25)] // 1875 + 1837.5 + 1818.75 = 5531.25
    [InlineData(CoverType.ContainerShip, 1.3, 1, 4793.75)] // 1625 + 1592.5 + 1576.25 = 4793.75
    public void ComputePremium_ShouldCalculateCorrectly_ForOneDay(CoverType type, decimal multiplier, int days,
        decimal expected)
    {
        _strategyMock.Setup(s => s.GetMultiplier()).Returns(multiplier);
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(days);

        var result = _coversService.ComputePremium(startDate, endDate, type);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ComputePremium_Yacht_35Days_CheckLogic()
    {
        // Yacht multiplier 1.1
        // Day 0-29 (30 days): 1250 * 1.1 = 1375
        // Day 30-34 (5 days): 
        //   i < 180 && Yacht: 1375 - (1375 * 0.05) = 1375 - 68.75 = 1306.25
        //   i < 365 && Yacht: 1306.25 (wait, the logic is additive?)

        // Let's re-examine the logic:
        /*
        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }
        */
        // If i=0:
        // totalPremium += 1375 (if < 30)
        // totalPremium += 1375 - 68.75 = 1306.25 (if Yacht and < 180)
        // totalPremium += 1375 - 110 = 1265 (if Yacht and < 365)
        // Total for day 0 = 1375 + 1306.25 + 1265 = 3946.25? 
        // This looks like a bug in the code or very strange premium calculation.
        // Usually it should be 'if / else if'.

        // Let's test what it CURRENTLY does.
        _strategyMock.Setup(s => s.GetMultiplier()).Returns(1.1m);
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(1);

        var result = _coversService.ComputePremium(startDate, endDate, CoverType.Yacht);

        // Day 0: 1375 + (1375 * 0.95) + (1375 * 0.92) = 1375 + 1306.25 + 1265 = 3946.25
        Assert.Equal(3946.25m, result);
    }

    [Fact]
    public async Task GetCoversAsync_ShouldReturnList()
    {
        var covers = new List<Cover>
        {
            new Cover
            {
                Id = Guid.CreateVersion7().ToString(),
                StartDate = DateTimeExtensions.UtcToday().AddDays(10),
                EndDate = DateTimeExtensions.UtcToday().AddDays(100),
                Type = CoverType.Yacht,
                Premium = 12345m
            },
            new Cover
            {
                Id = Guid.CreateVersion7().ToString(),
                StartDate = DateTimeExtensions.UtcToday().AddDays(2),
                EndDate = DateTimeExtensions.UtcToday().AddDays(33),
                Type = CoverType.Yacht,
                Premium = 20000m
            }
        };
        _coversRepositoryMock.Setup(r => r.GetCoversAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(covers);

        var result = await _coversService.GetCoversAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        _coversRepositoryMock.Verify(r => r.GetCoversAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCoverAsync_ShouldReturnCover()
    {
        var id = Guid.CreateVersion7().ToString();
        var cover = new Cover
        {
            Id = id,
            StartDate = DateTimeExtensions.UtcToday().AddDays(10),
            EndDate = DateTimeExtensions.UtcToday().AddDays(100),
            Type = CoverType.Yacht,
            Premium = 12345m
        };
        _coversRepositoryMock.Setup(r => r.GetCoverAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        var result = await _coversService.GetCoverAsync("1", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(12345m, result.Premium);
        Assert.Equal(CoverType.Yacht, result.Type);
    }

    [Fact]
    public async Task CreateCoverAsync_ShouldCreateAndAudit()
    {
        var model = new CreateCoverRequestModel
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(10),
            Type = CoverType.Yacht
        };
        _strategyMock.Setup(s => s.GetMultiplier()).Returns(1.1m);

        var result = await _coversService.CreateCoverAsync(model, CancellationToken.None);

        Assert.NotNull(result);
        _coversRepositoryMock.Verify(r => r.CreateCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _auditerMock.Verify(a => a.AuditCover(It.IsAny<string>(), "POST", It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(CoverType.Yacht, result.Type);
        Assert.NotEmpty(result.Id);
    }

    [Fact]
    public async Task DeleteCoverAsync_ShouldDeleteAndAudit()
    {
        var id = Guid.CreateVersion7().ToString();
        _coversRepositoryMock.Setup(r => r.DeleteCoverAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _coversService.DeleteCoverAsync(id, CancellationToken.None);

        _coversRepositoryMock.Verify(r => r.DeleteCoverAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _auditerMock.Verify(a => a.AuditCover(id, "DELETE", It.IsAny<CancellationToken>()), Times.Once);
    }
}