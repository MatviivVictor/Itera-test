using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class YachtMultiplierStrategy: IComputePremiumMultiplierStategy
{
    public decimal GetMultiplier()
    {
        return 1.1m;
    }
}