namespace MetadataService.Infrastructure.Metrics;

public interface IMetrics
{
    void IncSuccess(Type eventType);
    
    void IncError(Type eventType);
}