using Claims.Application.Interfaces;

namespace Claims.Application.Strategies;

public class PassengerShipMultiplierStrategy : ICoverTypeComputePremiumStategy
{
    public decimal GetExpenciveMultiplier()
    {
        return 1.2m;
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