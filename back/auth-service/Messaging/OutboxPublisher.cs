using System.Text.Json;
using AuthService.Data;
using MassTransit;

namespace AuthService.Messaging;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider      _sp;
    private readonly ILogger<OutboxPublisher> _log;
    private readonly TimeSpan              _delay = TimeSpan.FromSeconds(5);

    public OutboxPublisher(IServiceProvider sp, ILogger<OutboxPublisher> log)
    {
        _sp  = sp;      // нужен, чтобы внутри цикла брать новый DbContext
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

    /// <summary>
    ///  1. Берём пачку ≤100 НЕ-отправленных сообщений.<br/>
    ///  2. Публикуем каждое в шину.<br/>
    ///  3. Помечаем <c>Processed=true</c> &amp; сохраняем транзакцию.
    /// </summary>
    private async Task PublishPendingAsync(CancellationToken ct)
    {
        using var scope   = _sp.CreateScope();                     // unit of work
        var db            = scope.ServiceProvider.GetRequiredService<AuthContext>();
        var bus           = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var pending = await db.OutboxMessages
                              .Where(m => !m.Processed)
                              .OrderBy(m => m.Id)                   // FIFO
                              .Take(100)
                              .ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var msg in pending)
        {
            // 1. Получить runtime-тип события из строки Type
            var eventType = Type.GetType(msg.Type, throwOnError: false);
            if (eventType is null)
            {
                _log.LogWarning("Unknown outbox Type={Type}, skip id={Id}", msg.Type, msg.Id);
                msg.Processed = true;               // чтобы не застрять навсегда
                continue;
            }

            // 2. Десериализовать payload в объект
            object? evt = JsonSerializer.Deserialize(msg.Payload, eventType);
            if (evt is null)
            {
                _log.LogWarning("Failed to deserialize payload for id={Id}", msg.Id);
                msg.Processed = true;
                continue;
            }

            // 3. Опубликовать через MassTransit
            await bus.Publish(evt, eventType, ct);

            // 4. Отметить как выполнено
            msg.Processed = true;
        }

        await db.SaveChangesAsync(ct);
        _log.LogInformation("✔︎ Published {Count} outbox events", pending.Count);
    }
}