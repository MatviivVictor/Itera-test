namespace Claims.Application.Interfaces;

public interface ICoverTypeComputePremiumStategy
{
    /// <summary>
    /// Return a multiplier for the cover type.
    /// ex: return 1.1m for '10% more expensive' for Yacht (ex)
    /// </summary>
    /// <returns>
    /// A decimal value representing the multiplier to apply for premium calculation
    /// based on the associated cover type.
    /// </returns>
    decimal GetExpenciveMultiplier();

    /// <summary>
    /// Returns Discount for Base rate for the next 150 days
    /// ex: return 0.05m for 5% discount for Yacht (ex)
    /// </summary>
    /// <returns>A decimal value</returns>
    decimal Get150DaysDiscount();
    
    /// <summary>
    /// Return Additional Discount for the remaining 30 days
    /// ex: return 0.03m for 32% discount for Yacht (ex)
    /// </summary>
    /// <returns>A decimal value</returns>
    decimal GetAdditionalDiscount();
}