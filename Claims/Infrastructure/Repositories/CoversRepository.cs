using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using Claims.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class CoversRepository : ICoversRepository
{
    private readonly ClaimsContext _claimsContext;

    public CoversRepository(ClaimsContext claimsContext)
    {
        _claimsContext = claimsContext;
    }

    public Task<List<Cover>> GetCoversAsync(CancellationToken cancellationToken)
    {
        return _claimsContext.Covers.ToListAsync(cancellationToken);
    }

    public Task<Cover?> GetCoverAsync(string id, CancellationToken cancellationToken)
    {
        return _claimsContext.Covers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task CreateCoverAsync(Cover cover, CancellationToken cancellationToken)
    {
        _claimsContext.Covers.Add(cover);
        return _claimsContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteCoverAsync(string id, CancellationToken cancellationToken)
    {
        var result = false;
        var cover = await _claimsContext.Covers.Where(cover => cover.Id == id).SingleOrDefaultAsync(cancellationToken);
        if (cover is not null)
        {
            _claimsContext.Covers.Remove(cover);
            await _claimsContext.SaveChangesAsync(cancellationToken);
            result = true;
        }

        return result;
    }
}