using Claims.Domain.Entities;

namespace Claims.Domain.Interfaces;

public interface IClaimsRepository
{
    Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken);
    Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken);
    Task AddItemAsync(Claim item, CancellationToken cancellationToken);
    Task<bool> DeleteItemAsync(string id, CancellationToken cancellationToken);
}