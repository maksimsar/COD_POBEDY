namespace MetadataService.Infrastructure.Telemetry;

public interface IMetrics
{
    void IncSuccess(Type eventType);
    void IncError(Type eventType);
}

public sealed class NullMetrics : IMetrics
{
    public void IncSuccess(Type _) { }
    public void IncError(Type _)   { }
}