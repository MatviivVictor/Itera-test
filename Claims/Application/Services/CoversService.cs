using Claims.Application.Factories;
using Claims.Application.Interfaces;
using Claims.Application.Models;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;

namespace Claims.Application.Services;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditer _auditer;
    Func<CoverType, IComputePremiumMultiplierStategy> _getStrategy;

    public CoversService(ICoversRepository coversRepository, IAuditer auditer,
        Func<CoverType, IComputePremiumMultiplierStategy> getStrategy)
    {
        _coversRepository = coversRepository;
        _auditer = auditer;
        _getStrategy = getStrategy;
    }

    public async Task<List<CoverModel>> GetCoversAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversRepository.GetCoversAsync(cancellationToken);
        return covers.Select(CoverFactory.Create).ToList();
    }

    public async Task<CoverModel?> GetCoverAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coversRepository.GetCoverAsync(id, cancellationToken);
        if (cover is null) return null;
        return CoverFactory.Create(cover);
    }

    public async Task<CoverModel> CreateCoverAsync(CreateCoverRequestModel model, CancellationToken cancellationToken)
    {
        var cover = CoverFactory.Create(model);
        cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _coversRepository.CreateCoverAsync(cover, cancellationToken);
        await _auditer.AuditCover(cover.Id, "POST", cancellationToken);
        return CoverFactory.Create(cover);
    }

    public async Task DeleteCoverAsync(string id, CancellationToken cancellationToken)
    {
        var success = await _coversRepository.DeleteCoverAsync(id, cancellationToken);
        if (success)
        {
            await _auditer.AuditCover(id, "DELETE", cancellationToken);
        }
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var strategy = _getStrategy(coverType);

        var multiplier = strategy.GetMultiplier();

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30) totalPremium += premiumPerDay;
            if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
        }

        return totalPremium;
    }
}