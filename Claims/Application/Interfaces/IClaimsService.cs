using Claims.Application.Models;

namespace Claims.Application.Interfaces;

public interface IClaimsService
{
    Task<List<ClaimModel>> GetClaimsAsync(CancellationToken cancellationToken);
    Task<ClaimModel> CreateClaimAsync(CreateClaimRequestModel model, string post, CancellationToken cancellationToken);
    Task RemoveClaimAsync(string id, CancellationToken cancellationToken);
    Task<ClaimModel?> GetClaimAsync(string id, CancellationToken cancellationToken);
}