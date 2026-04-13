using Claims.Application.Extensions;
using Claims.Application.Models;
using Claims.Domain.Entities;

namespace Claims.Application.Factories;

public static class ClaimFactory
{
    public static ClaimModel Create(Claim claim) => new()
    {
        Id = claim.Id,
        CoverId = claim.CoverId,
        Created = claim.Created,
        Name = claim.Name,
        Type = claim.Type,
        DamageCost = claim.DamageCost
    };
    
    public static Claim Create(CreateClaimRequestModel model) => new()
    {
        Id = Guid.CreateVersion7().ToString(),
        CoverId = model.CoverId,
        Created = model.Created.UtcDate(),
        Name = model.Name,
        Type = model.Type,
        DamageCost = model.DamageCost
    };
    
}