using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class YachtMultiplierStrategy: ICoverTypeComputePremiumStategy
{
    public decimal GetExpensivePercentage()
    {
        return 0.1m;
    }

    public decimal Get150DaysDiscount()
    {
        return 0.05m;
    }

    public decimal GetAdditionalDiscount()
    {
        return 0.03m;
    }
}