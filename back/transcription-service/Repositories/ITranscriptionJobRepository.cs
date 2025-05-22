using TranscriptionService.Models;

namespace TranscriptionService.Repositories;

public interface ITranscriptionJobRepository
{
    Task<TranscriptionJob?> GetAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<TranscriptionJob>> GetByStatusAsync(string status, CancellationToken ct = default);
    
    void Add(TranscriptionJob job);
    void Update(TranscriptionJob job);
    
    Task SaveAsync(CancellationToken ct = default);
    
}