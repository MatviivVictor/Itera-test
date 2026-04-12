using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;

namespace Claims.Application.Services;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditer _auditer;
    Func<CoverType, IComputePremiumMultiplierStategy> _getStrategy;
    public CoversService(ICoversRepository coversRepository, IAuditer auditer, Func<CoverType, IComputePremiumMultiplierStategy> getStrategy)
    {
        _coversRepository = coversRepository;
        _auditer = auditer;
        _getStrategy = getStrategy;
    }

    public Task<List<Cover>> GetCoversAsync(CancellationToken cancellationToken)
    {
        return _coversRepository.GetCoversAsync(cancellationToken);
    }

    public Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken)
    {
        return _coversRepository.GetCoverAsync(id, cancellationToken);
    }

    public async Task<Cover> CreateCoverAsync(Cover cover, CancellationToken cancellationToken)
    {
        cover.Id = Guid.CreateVersion7().ToString();
        cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _coversRepository.CreateCoverAsync(cover, cancellationToken);
        await _auditer.AuditCover(cover.Id, "POST", cancellationToken);
        return cover;
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