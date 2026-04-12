using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class TankerMultiplierStrategy: IComputePremiumMultiplierStategy
{
    public decimal GetMultiplier()
    {
        return 1.5m;
    }
}