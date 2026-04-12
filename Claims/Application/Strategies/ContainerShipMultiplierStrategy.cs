using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class ContainerShipMultiplierStrategy: IComputePremiumMultiplierStategy
{
    public decimal GetMultiplier()
    {
        return 1.3m;
    }
}