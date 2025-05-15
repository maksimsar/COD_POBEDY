using MetadataService.DTOs;

namespace MetadataService.Application.Services;

public interface ITranscriptService
{
    Task<IReadOnlyList<TranscriptDto>> ListBySongAsycn(Guid songId, CancellationToken ct = default);
    
    Task ApproveAsync(long transcriptId, CancellationToken ct = default);
    
    Task UpdateTextAsync(long transcriptId, UpdateTranscriptRequest request, CancellationToken ct = default);
    
    Task DeleteAsync(long transcriptId, CancellationToken ct = default);
}