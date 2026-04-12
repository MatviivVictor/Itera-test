using Claims.Application.Models;
using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<List<CoverModel>> GetCoversAsync(CancellationToken cancellationToken);
    Task<CoverModel?> GetCoverAsync(string id, CancellationToken cancellationToken);
    Task<CoverModel> CreateCoverAsync(CreateCoverRequestModel model, CancellationToken cancellationToken);
    Task DeleteCoverAsync(string id, CancellationToken cancellationToken);
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}