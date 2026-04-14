using Claims.Application.Models;
using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

public interface IPremiumService
{
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    decimal ComputePremium(PremiumComputeRequestModel request);
}