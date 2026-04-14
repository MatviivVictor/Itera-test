using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class TankerMultiplierStrategy : ICoverTypeComputePremiumStategy
{
    public decimal GetExpensivePercentage()
    {
        return 0.5m;
    }

    public decimal Get150DaysDiscount()
    {
        return 0.02m;
    }

    public decimal GetAdditionalDiscount()
    {
        return 0.01m;
    }
}