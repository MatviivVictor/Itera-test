using Claims.Application.Interfaces;
using Claims.Domain.Entities;

namespace Claims.Application.Models;

/// <summary>
/// Represents a request model for creating a cover entity.
/// </summary>
/// <remarks>
/// The <c>CreateCoverRequestModel</c> class is used to encapsulate the details required
/// for creating a cover. It provides properties to specify the cover's start date,
/// end date, and type. This class implements the <see cref="ICoverPeriod"/> interface
/// to ensure compatibility with cover period validation.
/// </remarks>
public class PremiumComputeRequestModel : ICoverPeriod
{
    /// <summary>
    /// Gets or sets the start date of the coverage period.
    /// </summary>
    /// <remarks>
    /// The <c>StartDate</c> property specifies the date on which the insurance coverage begins.
    /// It is a mandatory field that must represent a valid date, and it cannot be in the past during validation.
    /// This property is used in premium computation and other processes where the coverage period is relevant.
    /// </remarks>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the coverage period.
    /// </summary>
    /// <remarks>
    /// The <c>EndDate</c> property denotes the date on which the insurance coverage terminates.
    /// It must represent a valid date and is critical in determining the total duration of the insurance period.
    /// Validation ensures that <c>EndDate</c> does not precede the <c>StartDate</c> and that the insurance period
    /// does not exceed the maximum allowed duration, typically one year.
    /// This property is used in premium calculation and validation processes.
    /// </remarks>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the type of coverage for the insurance policy.
    /// </summary>
    /// <remarks>
    /// The <c>Type</c> property specifies the category of insurance coverage, which determines
    /// the type of vehicle or vessel being insured. Possible values are defined in the <see cref="CoverType"/> enumeration.
    /// This property is a key determinant in premium computation as different coverage types may have varying premium rates.
    /// </remarks>
    public CoverType Type { get; set; }
}