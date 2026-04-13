using Claims.Domain.Interfaces;

namespace Claims.Infrastructure.Auditing
{
    public class Auditer: IAuditer
    {
        private readonly IAuditQueue _auditQueue;

        public Auditer(IAuditQueue auditQueue)
        {
            _auditQueue = auditQueue;
        }

        public Task AuditClaim(string id, string httpRequestType, CancellationToken cancellationToken )
        {
            return _auditQueue.EnqueueClaimAudit(id, httpRequestType).AsTask();
        }
        
        public Task AuditCover(string id, string httpRequestType, CancellationToken cancellationToken)
        {
            return _auditQueue.EnqueueCoverAudit(id, httpRequestType).AsTask();
        }
    }
}
