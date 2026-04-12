using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

public interface ICoversService
{
    Task<List<Cover>> GetCoversAsync(CancellationToken cancellationToken);
    Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken);
    Task<Cover> CreateCoverAsync(Cover cover, CancellationToken cancellationToken);
    Task DeleteCoverAsync(string id, CancellationToken cancellationToken);
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}