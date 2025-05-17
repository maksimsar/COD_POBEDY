using Prometheus;

namespace MetadataService.Infrastructure.Metrics;

internal sealed class PrometheusMetrics : IMetrics
{
    private static readonly Counter SuccessCounter =
        Prometheus.Metrics.CreateCounter(
            "consumer_success_total",
            "Количество успешно обработанных сообщений.",
            new CounterConfiguration { LabelNames = new[] { "event" } });

    private static readonly Counter ErrorCounter =
        Prometheus.Metrics.CreateCounter(
            "consumer_error_total",
            "Количество сообщений, завершившихся ошибкой.",
            new CounterConfiguration { LabelNames = new[] { "event" } });

    public void IncSuccess(Type eventType)
        => SuccessCounter.WithLabels(eventType.Name).Inc();

    public void IncError(Type eventType)
        => ErrorCounter.WithLabels(eventType.Name).Inc();
}