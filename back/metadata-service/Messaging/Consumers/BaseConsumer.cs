using MassTransit;
using MetadataService.Infrastructure.Metrics;
using OpenTelemetry.Trace;

namespace MetadataService.Messaging.Consumers;

public abstract class BaseConsumer<T> : IConsumer<T>
    where T : class
{
    private readonly ILogger<BaseConsumer<T>> _log;
    private readonly Tracer                   _tracer;
    private readonly IMetrics                 _metrics; // ваш интерфейс метрик‑инкрементов

    protected BaseConsumer(ILogger<BaseConsumer<T>> log, Tracer tracer, IMetrics metrics)
    {
        _log     = log;
        _tracer  = tracer;
        _metrics = metrics;
    }

    public async Task Consume(ConsumeContext<T> ctx)
    {
        using var span = _tracer.StartActiveSpan(typeof(T).Name);
        try
        {
            await ValidateAsync(ctx);
            await HandleAsync(ctx);
            _metrics.IncSuccess(typeof(T));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to handle {Event}", typeof(T).Name);
            _metrics.IncError(typeof(T));
            throw; // чтобы MassTransit перекинул в retry / DLQ
        }
    }

    protected virtual Task ValidateAsync(ConsumeContext<T> ctx) => Task.CompletedTask;

    protected abstract Task HandleAsync(ConsumeContext<T> ctx);
}