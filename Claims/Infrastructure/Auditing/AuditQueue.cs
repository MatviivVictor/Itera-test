using System.Threading.Channels;

namespace Claims.Infrastructure.Auditing;

public interface IAuditQueue
{
    ValueTask EnqueueClaimAudit(string id, string httpRequestType);
    ValueTask EnqueueCoverAudit(string id, string httpRequestType);
    ChannelReader<AuditMessage> Reader { get; }
}

public abstract record AuditMessage;
public record ClaimAuditMessage(string Id, string HttpRequestType, DateTime Created) : AuditMessage;
public record CoverAuditMessage(string Id, string HttpRequestType, DateTime Created) : AuditMessage;

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
