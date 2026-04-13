using Claims.Domain.Entities;

namespace Claims.Application.Models;

public class CreateClaimRequestModel
{
    public string CoverId { get; set; }
    public DateTime Created { get; set; }
    public string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
}