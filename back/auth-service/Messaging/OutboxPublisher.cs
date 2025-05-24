using System.Text.Json;
using AuthService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Messaging;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider      _sp;
    private readonly ILogger<OutboxPublisher> _log;
    private readonly TimeSpan              _delay = TimeSpan.FromSeconds(5);

    public OutboxPublisher(IServiceProvider sp, ILogger<OutboxPublisher> log)
    {
        _sp  = sp;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("▶︎ Outbox-publisher started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox-publisher error, will retry in {Delay}", _delay);
            }

            await Task.Delay(_delay, stoppingToken);
        }
    }
    
    private async Task PublishPendingAsync(CancellationToken ct)
    {
        using var scope   = _sp.CreateScope();                    
        var db            = scope.ServiceProvider.GetRequiredService<AuthContext>();
        var bus           = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pending = await db.OutboxMessages
                              .Where(m => !m.Processed)
                              .OrderBy(m => m.Id)               
                              .Take(100)
                              .ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var msg in pending)
        {
            var eventType = Type.GetType(msg.Type, throwOnError: false);
            if (eventType is null)
            {
                _log.LogWarning("Unknown outbox Type={Type}, skip id={Id}", msg.Type, msg.Id);
                msg.Processed = true; 
                continue;
            }
            
            object? evt = JsonSerializer.Deserialize(msg.Payload, eventType!, new JsonSerializerOptions());;
            if (evt is null)
            {
                _log.LogWarning("Failed to deserialize payload for id={Id}", msg.Id);
                msg.Processed = true;
                continue;
            }
            
            await bus.Publish(evt, eventType, ct);
            
            msg.Processed = true;
        }

        await db.SaveChangesAsync(ct);
        _log.LogInformation("✔︎ Published {Count} outbox events", pending.Count);
    }
}