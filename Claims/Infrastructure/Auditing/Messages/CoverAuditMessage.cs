namespace Claims.Infrastructure.Auditing.Messages;

public record CoverAuditMessage(string Id, string HttpRequestType, DateTime Created) : AuditMessage;