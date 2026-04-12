using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class BulkCarrierMultiplierStrategy: IComputePremiumMultiplierStategy
{
    public decimal GetMultiplier()
    {
        return 1.3m;
    }
}