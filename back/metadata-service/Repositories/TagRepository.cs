using MetadataService.Data;
using MetadataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MetadataService.Repositories;

internal sealed class TagRepository : RepositoryBase<Tag>, ITagRepository
{
    public TagRepository(MetadataContext context) : base(context) { }

    public Task<Tag?> GetByNameAsync(string name, CancellationToken ct = default) =>
        _set.FirstOrDefaultAsync(t => t.Name == name, ct);
}