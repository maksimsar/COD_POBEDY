using MetadataService.DTOs;

namespace MetadataService.Application.Services;

public interface ISongService
{
    Task<SongDto?> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<IReadOnlyList<SongDto>> ListAsync(
        short?  year      = null,
        Guid?   authorId  = null,
        int?    tagId     = null,
        CancellationToken ct = default);
    
    Task<Guid> CreateAsync(CreateSongRequest request, CancellationToken ct = default);
    
    Task UpdateAsync(Guid id, UpdateSongRequest request, CancellationToken ct = default);
    
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    
    Task UpdateTagsAsync(UpdateTagsRequest request, CancellationToken ct = default);
}