using Claims.Domain.Entities;

namespace Claims.Application.Models;

/// <summary>
/// Represents a model used to create a new claim request in the claims management system.
/// </summary>
public class CreateClaimRequestModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the cover associated with the claim.
    /// </summary>
    /// <remarks>
    /// This property represents the cover ID used to link a claim to its associated insurance coverage.
    /// It is required and must not be empty.
    /// </remarks>
    public string CoverId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the creation date of the claim request.
    /// </summary>
    /// <remarks>
    /// This property represents the date indicating when the claim request was created.
    /// It is used for auditing purposes and ensuring the claim falls within the valid coverage period.
    /// </remarks>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the name associated with the claim request.
    /// </summary>
    /// <remarks>
    /// This property represents the descriptive or identifying name provided for the claim.
    /// It is used to associate a label or title with the claim being created.
    /// </remarks>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of claim being created.
    /// </summary>
    /// <remarks>
    /// This property represents the category of the claim, such as Collision, Grounding,
    /// BadWeather, or Fire. It must hold a valid value from the <see cref="ClaimType"/> enumeration.
    /// </remarks>
    public ClaimType Type { get; set; }

    /// <summary>
    /// Gets or sets the estimated cost of the damage associated with the claim.
    /// </summary>
    /// <remarks>
    /// This property represents the monetary amount attributed to the damage being claimed.
    /// Validation ensures the cost does not exceed the allowed maximum limit.
    /// </remarks>
    public decimal DamageCost { get; set; }
}