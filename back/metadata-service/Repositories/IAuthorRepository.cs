using MetadataService.Models;

namespace MetadataService.Repositories;

public interface IAuthorRepository : IRepository<Author>
{
    Task<bool> ExistsAsync(string fullName, CancellationToken ct = default);
}