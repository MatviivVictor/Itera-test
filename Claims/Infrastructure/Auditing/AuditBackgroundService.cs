using Claims.Auditing;
using Claims.Infrastructure.Auditing.Messages;
using Claims.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Auditing;

public class AuditBackgroundService : BackgroundService
{
    private readonly IAuditQueue _auditQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditBackgroundService> _logger;
    private readonly int _maxRetries = 3;

    public AuditBackgroundService(IAuditQueue auditQueue, IServiceProvider serviceProvider,
        ILogger<AuditBackgroundService> logger)
    {
        _auditQueue = auditQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Audit background service is starting.");

        await foreach (var message in _auditQueue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var passed = false;
                var retries = 0;

                while (!passed && retries < _maxRetries)
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                        switch (message)
                        {
                            case ClaimAuditMessage claimMsg:
                                // check entry already exists
                                var existingClaimAudit = await dbContext.ClaimAudits.AnyAsync(
                                    x => x.ClaimId == claimMsg.Id && x.Created == claimMsg.Created &&
                                         x.HttpRequestType == claimMsg.HttpRequestType, stoppingToken);
                                
                                if (existingClaimAudit)
                                {
                                    _logger.LogInformation(
                                        "Claim audit for {ClaimId} at {Created} already exists. Skipping.",
                                        claimMsg.Id, claimMsg.Created);
                                    passed = true;
                                    break;
                                }

                                var claimAudit = new ClaimAudit
                                {
                                    ClaimId = claimMsg.Id,
                                    HttpRequestType = claimMsg.HttpRequestType,
                                    Created = claimMsg.Created
                                };
                                dbContext.Add(claimAudit);
                                break;
                            case CoverAuditMessage coverMsg:
                                // check entry already exists
                                var existingCoverAudit = await dbContext.CoverAudits.AnyAsync(
                                    x => x.CoverId == coverMsg.Id && x.Created == coverMsg.Created &&
                                         x.HttpRequestType == coverMsg.HttpRequestType, stoppingToken);

                                if (existingCoverAudit)
                                {
                                    _logger.LogInformation(
                                        "Cover audit for {CoverId} at {Created} already exists. Skipping.",
                                        coverMsg.Id, coverMsg.Created);
                                    passed = true;
                                    break;
                                }

                                var coverAudit = new CoverAudit
                                {
                                    CoverId = coverMsg.Id,
                                    HttpRequestType = coverMsg.HttpRequestType,
                                    Created = coverMsg.Created
                                };
                                dbContext.Add(coverAudit);
                                break;
                            default:
                                _logger.LogWarning("Unknown audit message type: {MessageType}", message.GetType().Name);
                                passed = true; // No retrying when unknown type
                                break;
                        }

                        if (passed) break;

                        await dbContext.SaveChangesAsync(stoppingToken);
                        passed = true;
                    }
                    catch (Exception ex) when (retries < _maxRetries - 1 && !stoppingToken.IsCancellationRequested)
                    {
                        retries++;
                        _logger.LogWarning(ex, "Error processing audit message. Retry {RetryCount} of {MaxRetries}.",
                            retries, _maxRetries);
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retries)), stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing audit message.");
            }
        }

        _logger.LogInformation("Audit background service is stopping.");
    }
}