using Claims.Application.Factories;
using Claims.Application.Interfaces;
using Claims.Application.Models;
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

    public async Task<List<ClaimModel>> GetClaimsAsync(CancellationToken cancellationToken)
    {
        var claims = await _claimsRepository.GetClaimsAsync(cancellationToken);

        return claims.Select(ClaimFactory.Create).ToList();
    }

    public async Task<ClaimModel> CreateClaimAsync(CreateClaimRequestModel model, string post,
        CancellationToken cancellationToken)
    {
        var claim = ClaimFactory.Create(model);
        var cover = await _coversRepository.GetCoverAsync(claim.CoverId, cancellationToken);
        ValidateClaimCreatedDate(cover, claim);

        await _claimsRepository.AddItemAsync(claim, cancellationToken);
        await _auditer.AuditClaim(claim.Id, "POST", cancellationToken);
        return ClaimFactory.Create(claim);
    }

    private static void ValidateClaimCreatedDate(Cover? cover, Claim claim)
    {
        var validator = new ClaimValidator(cover);
        var validatorResult = validator.Validate(claim);
        if (!validatorResult.IsValid)
        {
            throw new ValidationException(validatorResult.Errors);
        }
    }

    public async Task RemoveClaimAsync(string id, CancellationToken cancellationToken)
    {
        var success = await _claimsRepository.DeleteItemAsync(id, cancellationToken);
        if (success)
        {
            await _auditer.AuditClaim(id, "DELETE", cancellationToken);
        }
    }

    public async Task<ClaimModel?> GetClaimAsync(string id, CancellationToken cancellationToken)
    {
        var claim = await _claimsRepository.GetClaimAsync(id, cancellationToken);
        
        if (claim is null)
            return null;
        
        return ClaimFactory.Create(claim);
    }
}