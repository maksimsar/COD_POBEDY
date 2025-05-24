using TranscriptionService.DTOs;

namespace TranscriptionService.Services;

public interface ITranscriptionService
{
    Task ProcessAsync(long jobId,
        string storageKey,
        CancellationToken ct = default);
    
    Task RetryAsync(long jobId, CancellationToken ct = default);
    
    Task<TranscriptionJobDto?> GetAsync(long jobId, CancellationToken ct = default);
}