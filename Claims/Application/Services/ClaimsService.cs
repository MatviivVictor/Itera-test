using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;

namespace Claims.Application.Services;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly IAuditer _auditer;

    public ClaimsService(IClaimsRepository claimsRepository, IAuditer auditer)
    {
        _claimsRepository = claimsRepository;
        _auditer = auditer;
    }

    public Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
    {
        return _claimsRepository.GetClaimsAsync(cancellationToken);
    }

    public async Task<Claim> CreateClaimAsync(Claim claim, string post, CancellationToken cancellationToken)
    {
        claim.Id = Guid.CreateVersion7().ToString();
        await _claimsRepository.AddItemAsync(claim, cancellationToken);
        await _auditer.AuditClaim(claim.Id, "POST", cancellationToken);
        return claim;
    }

    public async Task RemoveClaimAsync(string id, CancellationToken cancellationToken)
    {
        var success = await _claimsRepository.DeleteItemAsync(id, cancellationToken);
        if (success)
        {
            await _auditer.AuditClaim(id, "DELETE", cancellationToken);
        }
    }

    public Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken)
    {
        return _claimsRepository.GetClaimAsync(id, cancellationToken);
    }
}