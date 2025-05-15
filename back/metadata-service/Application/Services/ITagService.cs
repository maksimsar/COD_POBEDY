using MetadataService.DTOs;

namespace MetadataService.Application.Services;

public interface ITagService
{
    Task<TagDto> GetAsync(int id, CancellationToken ct = default);
    
    Task<IReadOnlyList<TagDto>> ListAsync(CancellationToken ct = default);
    
    Task<int> CreateAsync(CreateTagRequest request, CancellationToken ct = default);
    
    Task ApproveAsync(int id, CancellationToken ct = default);
    
    Task UpdateAsync(int id, UpdateTagRequest request, CancellationToken ct = default);
    
    Task DeleteAsync(int id, CancellationToken ct = default);
}