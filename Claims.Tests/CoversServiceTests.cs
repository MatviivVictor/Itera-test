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
    private readonly Mock<IPremiumService> _premiumServiceMock;
    private readonly CoversService _coversService;

    public CoversServiceTests()
    {
        _premiumServiceMock = new Mock<IPremiumService>();
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _auditerMock = new Mock<IAuditer>();

        _coversService = new CoversService(
            _coversRepositoryMock.Object,
            _auditerMock.Object, _premiumServiceMock.Object);
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
        var premiumValue = 12345m;
        var model = new CreateCoverRequestModel
        {
            StartDate = DateTimeExtensions.UtcToday(),
            EndDate = DateTimeExtensions.UtcToday().AddDays(10),
            Type = CoverType.Yacht
        };
        _premiumServiceMock.Setup(p => p.ComputePremium(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CoverType>())).Returns(premiumValue);

        var result = await _coversService.CreateCoverAsync(model, CancellationToken.None);

        Assert.NotNull(result);
        _coversRepositoryMock.Verify(r => r.CreateCoverAsync(It.IsAny<Cover>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _auditerMock.Verify(a => a.AuditCover(It.IsAny<string>(), "POST", It.IsAny<CancellationToken>()), Times.Once);
        _premiumServiceMock.Verify(p => p.ComputePremium(model.StartDate, model.EndDate, model.Type), Times.Once);
        
        Assert.Equal(CoverType.Yacht, result.Type);
        Assert.NotEmpty(result.Id);
        Assert.Equal(premiumValue, result.Premium);
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