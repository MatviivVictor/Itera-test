using Claims.Domain.Entities;

namespace Claims.Domain.Interfaces;

public interface ICoversRepository
{
    Task<List<Cover>> GetCoversAsync(CancellationToken cancellationToken);
    Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken);
    Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken);
    Task<bool> DeleteCoverAsync(string id, CancellationToken cancellationToken);
}