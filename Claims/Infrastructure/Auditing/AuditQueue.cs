using System.Threading.Channels;
using Claims.Infrastructure.Auditing.Messages;

namespace Claims.Infrastructure.Auditing;

public class AuditQueue : IAuditQueue
{
    private readonly Channel<AuditMessage> _channel;

    public AuditQueue()
    {
        // Drops the oldest message when full as a default strategy for auditing, 
        // to avoid blocking the main application logic.
        _channel = Channel.CreateBounded<AuditMessage>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });
    }

    public ValueTask EnqueueClaimAudit(string id, string httpRequestType)
    {
        return _channel.Writer.WriteAsync(new ClaimAuditMessage(id, httpRequestType, DateTime.UtcNow));
    }

    public ValueTask EnqueueCoverAudit(string id, string httpRequestType)
    {
        return _channel.Writer.WriteAsync(new CoverAuditMessage(id, httpRequestType, DateTime.UtcNow));
    }

    public ChannelReader<AuditMessage> Reader => _channel.Reader;
}
