using Claims.Application.Extensions;
using Claims.Application.Models;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using Moq;
using Xunit;
using FluentValidation;

namespace Claims.Tests;

public class ClaimsServiceTests
{
    private readonly Mock<IClaimsRepository> _claimsRepositoryMock;
    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly Mock<IAuditer> _auditerMock;
    private readonly ClaimsService _claimsService;

    public ClaimsServiceTests()
    {
        _claimsRepositoryMock = new Mock<IClaimsRepository>();
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _auditerMock = new Mock<IAuditer>();

        _claimsService = new ClaimsService(
            _claimsRepositoryMock.Object,
            _auditerMock.Object,
            _coversRepositoryMock.Object);
    }

    [Fact]
    public async Task GetClaimsAsync_ShouldReturnList()
    {
        var claims = new List<Claim>
        {
            new Claim
            {
                Id = Guid.CreateVersion7().ToString(),
                CoverId = Guid.CreateVersion7().ToString(),
                Created = DateTimeExtensions.UtcToday(),
                Name = "Name 1",
                Type = ClaimType.Collision,
                DamageCost = 0
            },
            new Claim
            {
                Id = Guid.CreateVersion7().ToString(),
                CoverId = Guid.CreateVersion7().ToString(),
                Created = DateTimeExtensions.UtcToday(),
                Name = "Name 2",
                Type = ClaimType.Collision,
                DamageCost = 0
            }
        };
        _claimsRepositoryMock.Setup(r => r.GetClaimsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(claims);

        var result = await _claimsService.GetClaimsAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        _claimsRepositoryMock.Verify(r => r.GetClaimsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClaimAsync_ShouldCreateAndAudit_WhenValid()
    {
        var coverId = Guid.CreateVersion7().ToString();
        var model = new CreateClaimRequestModel
        {
            CoverId = coverId,
            Created = DateTimeExtensions.UtcToday(),
            DamageCost = 1000,
            Name = "Test Claim",
            Type = ClaimType.Fire
        };
        var cover = new Cover
        {
            Id = coverId,
            StartDate = DateTimeExtensions.UtcToday().AddDays(-10),
            EndDate = DateTimeExtensions.UtcToday().AddDays(10)
        };

        _coversRepositoryMock.Setup(r => r.GetCoverAsync(coverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        var result = await _claimsService.CreateClaimAsync(model, "POST", CancellationToken.None);

        Assert.NotNull(result);
        _claimsRepositoryMock.Verify(r => r.AddItemAsync(It.IsAny<Claim>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditerMock.Verify(a => a.AuditClaim(It.IsAny<string>(), "POST", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateClaimAsync_ShouldThrowValidationException_WhenOutsideCoverPeriod()
    {
        var coverId = Guid.CreateVersion7().ToString();
        var model = new CreateClaimRequestModel
        {
            CoverId = coverId,
            Created = DateTimeExtensions.UtcToday().AddDays(-20), // Outside period
            DamageCost = 1000,
            Name = "Test Claim",
            Type = ClaimType.Fire
        };
        var cover = new Cover
        {
            Id = coverId,
            StartDate = DateTimeExtensions.UtcToday().AddDays(-10),
            EndDate = DateTimeExtensions.UtcToday().AddDays(10)
        };

        _coversRepositoryMock.Setup(r => r.GetCoverAsync(coverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cover);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _claimsService.CreateClaimAsync(model, "POST", CancellationToken.None));
    }

    [Fact]
    public async Task RemoveClaimAsync_ShouldDeleteAndAudit_IfSuccess()
    {
        var id = Guid.CreateVersion7().ToString();
        _claimsRepositoryMock.Setup(r => r.DeleteItemAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _claimsService.RemoveClaimAsync(id, CancellationToken.None);

        _claimsRepositoryMock.Verify(r => r.DeleteItemAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _auditerMock.Verify(a => a.AuditClaim(id, "DELETE", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetClaimAsync_ShouldReturnClaim()
    {
        var id = Guid.CreateVersion7().ToString();
        
        var claim = new Claim
        {
            Id = id,
            CoverId = Guid.CreateVersion7().ToString(),
            Created = DateTimeExtensions.UtcToday(),
            Name = "Name 1",
            Type = ClaimType.Collision,
            DamageCost = 12345
        };
        _claimsRepositoryMock.Setup(r => r.GetClaimAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(claim);

        var result = await _claimsService.GetClaimAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Name 1", result.Name);
    }
}