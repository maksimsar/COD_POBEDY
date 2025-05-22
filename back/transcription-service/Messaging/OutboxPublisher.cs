using MassTransit;
using Microsoft.EntityFrameworkCore;
using TranscriptionService.Data;

namespace TranscriptionService.Messaging;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxPublisher> _log;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    public OutboxPublisher(IServiceProvider sp, ILogger<OutboxPublisher> log)
    {
        _sp  = sp;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db  = scope.ServiceProvider.GetRequiredService<TranscriptionContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var batch = await db.OutboxMessages
                    .Where(m => !m.Processed)
                    .OrderBy(m => m.Id)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var msg in batch)
                {
                    var type = Type.GetType(msg.Type, throwOnError: false);
                    if (type is null) continue;                       // не знаем такой тип

                    var payload = System.Text.Json.JsonSerializer.Deserialize(msg.Payload, type);
                    if (payload is null) continue;

                    await bus.Publish(payload, type, stoppingToken);
                    msg.Processed = true;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox publish failed, retry in 5s");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}