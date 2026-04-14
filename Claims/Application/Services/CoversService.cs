using Claims.Application.Factories;
using Claims.Application.Interfaces;
using Claims.Application.Models;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;

namespace Claims.Application.Services;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly IAuditer _auditer;
    private readonly IPremiumService _premiumService;

    public CoversService(ICoversRepository coversRepository, IAuditer auditer, IPremiumService premiumService)
    {
        _coversRepository = coversRepository;
        _auditer = auditer;
        _premiumService = premiumService;
    }

    public async Task<List<CoverModel>> GetCoversAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversRepository.GetCoversAsync(cancellationToken);
        return covers.Select(CoverFactory.Create).ToList();
    }

    public async Task<CoverModel?> GetCoverAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coversRepository.GetCoverAsync(id, cancellationToken);
        if (cover is null) return null;
        return CoverFactory.Create(cover);
    }

    public async Task<CoverModel> CreateCoverAsync(CreateCoverRequestModel model, CancellationToken cancellationToken)
    {
        var cover = CoverFactory.Create(model);
        cover.Premium = _premiumService.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _coversRepository.CreateCoverAsync(cover, cancellationToken);
        await _auditer.AuditCover(cover.Id, "POST", cancellationToken);
        return CoverFactory.Create(cover);
    }

    public async Task DeleteCoverAsync(string id, CancellationToken cancellationToken)
    {
        var success = await _coversRepository.DeleteCoverAsync(id, cancellationToken);
        if (success)
        {
            await _auditer.AuditCover(id, "DELETE", cancellationToken);
        }
    }
}