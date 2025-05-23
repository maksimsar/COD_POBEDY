using MetadataService.DTOs;

namespace MetadataService.Application.Services;

public interface IAuthorService
{
    Task<AuthorDto> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<IReadOnlyList<AuthorDto>> ListAsync(CancellationToken ct = default);
    
    Task<Guid> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default);
    
    Task UpdateAsync(Guid id, UpdateAuthorRequest request, CancellationToken ct = default);
    
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}