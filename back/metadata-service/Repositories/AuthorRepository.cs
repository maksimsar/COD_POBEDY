using MetadataService.Data;
using MetadataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MetadataService.Repositories;

internal sealed class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
{
    public AuthorRepository(MetadataContext context) : base(context) { }

    public Task<bool> ExistsAsync(string fullName, CancellationToken ct = default) =>
        _set.AnyAsync(a => a.FullName == fullName, ct);
}