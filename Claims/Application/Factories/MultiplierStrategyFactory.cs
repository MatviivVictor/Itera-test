using Claims.Application.Interfaces;
using Claims.Application.Strategies;
using Claims.Domain.Entities;

namespace Claims.Application.Factories;

public static class MultiplierStrategyFactory
{
    public static IComputePremiumMultiplierStategy GetStrategy(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => new YachtMultiplierStrategy(),
            CoverType.PassengerShip => new PassengerShipMultiplierStrategy(),
            CoverType.ContainerShip => new ContainerShipMultiplierStrategy(),
            CoverType.BulkCarrier => new BulkCarrierMultiplierStrategy(),
            CoverType.Tanker => new TankerMultiplierStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(coverType), coverType, null)
        };
    }
}