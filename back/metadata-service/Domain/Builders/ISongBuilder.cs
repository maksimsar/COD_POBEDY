using MetadataService.Models;

namespace MetadataService.Domain.Builders;

public interface ISongBuilder
{
    ISongBuilder For(Song song);
    
    Task<ISongBuilder> AttachAuthorsAsync(IReadOnlyList<Guid> authorIds, CancellationToken ct = default);
    
    Task<ISongBuilder> AttachTagsAsync(int[] tagIds, CancellationToken ct = default);
    
    ISongBuilder AddTranscriptSegment(Transcript segment);
    
    Task<Song> BuildAsync(CancellationToken ct = default);
}