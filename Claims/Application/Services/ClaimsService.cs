using Claims.Application.Interfaces;
using Claims.Application.Validators;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using FluentValidation;

namespace Claims.Application.Services;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditer _auditer;

    public ClaimsService(IClaimsRepository claimsRepository, IAuditer auditer, ICoversRepository coversRepository)
    {
        _claimsRepository = claimsRepository;
        _auditer = auditer;
        _coversRepository = coversRepository;
    }

    public Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
    {
        return _claimsRepository.GetClaimsAsync(cancellationToken);
    }

    public async Task<Claim> CreateClaimAsync(Claim claim, string post, CancellationToken cancellationToken)
    {
        claim.Id = Guid.CreateVersion7().ToString();
        var cover = await _coversRepository.GetCoverAsync(claim.CoverId, cancellationToken);
        var validator = new ClaimCreatedValidation(cover);
        var validatorResult = validator.Validate(claim);
        if (!validatorResult.IsValid)
        {
            throw new ValidationException(validatorResult.Errors);
        }
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