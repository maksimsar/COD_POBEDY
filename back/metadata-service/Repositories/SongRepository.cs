using MetadataService.Data;
using MetadataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MetadataService.Repositories;

internal sealed class SongRepository : UnitOfWorkRepositoryBase<Song>, ISongRepository
{
    public SongRepository(MetadataContext context) : base(context) { }

    public Task<Song?> GetFullAsync(Guid id, CancellationToken ct = default) =>
        _set
            .Include(s => s.SongAuthors).ThenInclude(sa => sa.Author)
            .Include(s => s.SongTags).ThenInclude(st => st.Tag)
            .Include(s => s.Transcripts)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
}