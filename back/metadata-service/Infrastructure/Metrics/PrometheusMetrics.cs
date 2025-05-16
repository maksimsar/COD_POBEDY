using Prometheus;

namespace MetadataService.Infrastructure.Metrics;

internal sealed class PrometheusMetrics : IMetrics
{
    private static readonly Counter SuccessCounter = Metrics
        .CreateCounter(
            name: "consumer_success_total",
            help: "Количество успешно обработанных сообщений.",
            configuration: new CounterConfiguration
            {
                LabelNames = new[] { "event" }
            });

    private static readonly Counter ErrorCounter = Metrics
        .CreateCounter(
            name: "consumer_error_total",
            help: "Количество сообщений, завершившихся ошибкой.",
            configuration: new CounterConfiguration
            {
                LabelNames = new[] { "event" }
            });

    public void IncSuccess(Type eventType)
        => SuccessCounter.WithLabels(eventType.Name).Inc();

    public void IncError(Type eventType)
        => ErrorCounter.WithLabels(eventType.Name).Inc();
}