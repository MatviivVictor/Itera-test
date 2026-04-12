using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class PassengerShipMultiplierStrategy: IComputePremiumMultiplierStategy
{
    public decimal GetMultiplier()
    {
        return 1.2m;
    }
}