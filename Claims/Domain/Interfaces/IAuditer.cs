namespace Claims.Domain.Interfaces;

public interface IAuditer
{
    Task AuditClaim(string id, string httpRequestType, CancellationToken cancellationToken);
    Task AuditCover(string id, string httpRequestType, CancellationToken cancellationToken);
}