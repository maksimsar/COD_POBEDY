using MetadataService.Models;

namespace MetadataService.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken ct = default);
}