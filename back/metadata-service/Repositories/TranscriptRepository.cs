using MetadataService.Data;
using MetadataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MetadataService.Repositories;

internal sealed class TranscriptRepository : RepositoryBase<Transcript>, ITranscriptRepository
{
    public TranscriptRepository(MetadataContext context) : base(context) { }

    public async Task<IReadOnlyList<Transcript>> ListBySongAsync(
        Guid songId,
        CancellationToken ct = default)
    {
        var list = await _set
            .Where(t => t.SongId == songId)
            .OrderBy(t => t.SegmentIndex)
            .AsNoTracking()
            .ToListAsync(ct);
    
        return list;
    }

}