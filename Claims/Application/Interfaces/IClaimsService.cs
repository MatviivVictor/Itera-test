using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken);
    Task<Claim> CreateClaimAsync(Claim claim, string post, CancellationToken cancellationToken);
    Task RemoveClaimAsync(string id, CancellationToken cancellationToken);
    Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken);
}