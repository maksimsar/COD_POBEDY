using TranscriptionService.Messaging.Contracts;
using TranscriptionService.Models;

namespace TranscriptionService.Repositories;

public interface ITranscriptionJobRepository
{
    Task<TranscriptionJob?> GetAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<TranscriptionJob>> GetByStatusAsync(string status, CancellationToken ct = default);
    
    void Add(TranscriptionJob job);
    void Update(TranscriptionJob job);
    
    void AddOutbox(OutboxMessage message);
    
    Task SaveAsync(CancellationToken ct = default);
    
}