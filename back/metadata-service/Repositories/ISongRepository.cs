using MetadataService.Models;

namespace MetadataService.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<Song?> GetFullAsync(Guid id, CancellationToken ct = default); // +Authors +Tags +Transcripts
}