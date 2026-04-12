using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using Claims.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

public class ClaimsRepository : IClaimsRepository
{
    private readonly ClaimsContext _claimsContext;

    public ClaimsRepository(ClaimsContext claimsContext)
    {
        _claimsContext = claimsContext;
    }

    public Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
    {
        return _claimsContext.Claims.ToListAsync(cancellationToken);
    }

    public Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken)
    {
        return _claimsContext.Claims
            .Where(claim => claim.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task AddItemAsync(Claim item, CancellationToken cancellationToken)
    {
        _claimsContext.Claims.Add(item);
        return _claimsContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteItemAsync(string id, CancellationToken cancellationToken)
    {
        var result = false;
        var claim = await GetClaimAsync(id, cancellationToken);
        if (claim is not null)
        {
            _claimsContext.Claims.Remove(claim);
            await _claimsContext.SaveChangesAsync(cancellationToken);
            result = true;
        }
        
        return result;
    }
}