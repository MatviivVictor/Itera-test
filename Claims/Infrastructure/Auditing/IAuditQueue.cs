using System.Threading.Channels;
using Claims.Infrastructure.Auditing.Messages;

namespace Claims.Infrastructure.Auditing;

public interface IAuditQueue
{
    ValueTask EnqueueClaimAudit(string id, string httpRequestType);
    ValueTask EnqueueCoverAudit(string id, string httpRequestType);
    ChannelReader<AuditMessage> Reader { get; }
}