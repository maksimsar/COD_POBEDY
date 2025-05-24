using MassTransit;
using TranscriptionService.Data;

namespace TranscriptionService.Messaging.Consumers;

public abstract class BaseConsumer<TMessage> : IConsumer<TMessage>
    where TMessage : class
{
    protected readonly ILogger _log;
    protected readonly TranscriptionContext _db;
    protected readonly IPublishEndpoint _bus;

    protected BaseConsumer(ILogger logger,
        TranscriptionContext db,
        IPublishEndpoint bus)
    {
        _log = logger;
        _db = db;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<TMessage> ctx)
    {
        try
        {
            await ValidateAsync(ctx.Message, ctx.CancellationToken);
            await HandleCoreAsync(ctx.Message, ctx.CancellationToken);
            await _db.SaveChangesAsync(ctx.CancellationToken);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while processing {MessageType}", typeof(TMessage).Name);
            throw;
        }
    }

    protected virtual Task ValidateAsync(TMessage msg, CancellationToken ct) => Task.CompletedTask;

    protected abstract Task HandleCoreAsync(TMessage msg, CancellationToken ct);
}