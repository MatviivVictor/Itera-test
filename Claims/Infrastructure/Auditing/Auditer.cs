using Claims.Auditing;
using Claims.Domain.Interfaces;
using Claims.Infrastructure.Database;

namespace Claims.Infrastructure.Auditing
{
    public class Auditer: IAuditer
    {
        private readonly AuditContext _auditContext;

        public Auditer(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public Task AuditClaim(string id, string httpRequestType, CancellationToken cancellationToken )
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            _auditContext.Add(claimAudit);
            return _auditContext.SaveChangesAsync(cancellationToken);
        }
        
        public Task AuditCover(string id, string httpRequestType, CancellationToken cancellationToken)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            _auditContext.Add(coverAudit);
            return _auditContext.SaveChangesAsync(cancellationToken);
        }
    }
}
