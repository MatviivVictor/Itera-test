namespace Claims.Infrastructure.Auditing.Messages;

public record ClaimAuditMessage(string Id, string HttpRequestType, DateTime Created) : AuditMessage;