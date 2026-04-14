using Claims.Application.Extensions;
using Claims.Application.Interfaces;
using Claims.Application.Models;
using Claims.Domain.Entities;

namespace Claims.Application.Services;

public class PremiumService : IPremiumService
{
    Func<CoverType, ICoverTypeComputePremiumStategy> _getStrategy;
    private readonly decimal _baseDayRate = 1250m;

    /// <summary>
    /// Computes the premium for a given cover period using the specified start date, end date, and cover type.
    /// </summary>
    /// <param name="startDate">The start date of the cover period.</param>
    /// <param name="endDate">The end date of the cover period.</param>
    /// <param name="coverType">The type of cover to calculate the premium for.</param>
    /// <returns>The calculated premium for the specified cover period.</returns>
    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        endDate = endDate.Min(startDate.AddYears(1));

        var strategy = _getStrategy(coverType);

        var dayRate = _baseDayRate + _baseDayRate * strategy.GetExpensivePercentage();
        var _150dayDiscount = strategy.Get150DaysDiscount();
        var remainingDaysDiscount = strategy.GetAdditionalDiscount();
        var coverPeriodDays = (endDate - startDate).Days;
        var totalPremium = 0m;
        var firstPeriodDaysCount = Math.Min(30, coverPeriodDays);
        totalPremium = dayRate * firstPeriodDaysCount;
        var secondPeriodDaysCount = Math.Min(Math.Max(coverPeriodDays - 30, 0), 150);
        totalPremium += (dayRate - dayRate * _150dayDiscount) * secondPeriodDaysCount;
        var thirdPeriodDaysCount = Math.Max(coverPeriodDays - 180, 0);
        totalPremium += (dayRate - dayRate * (_150dayDiscount + remainingDaysDiscount)) * thirdPeriodDaysCount;
        return totalPremium;
    }

    /// <summary>
    /// Computes the premium for a cover period based on the given start date, end date, and cover type.
    /// </summary>
    /// <param name="startDate">The start date of the cover period.</param>
    /// <param name="endDate">The end date of the cover period.</param>
    /// <param name="coverType">The type of cover to calculate the premium for.</param>
    /// <returns>The calculated premium for the cover period.</returns>
    public decimal ComputePremium(PremiumComputeRequestModel request)
    {
        return ComputePremium(request.StartDate, request.EndDate, request.Type);
    }
}