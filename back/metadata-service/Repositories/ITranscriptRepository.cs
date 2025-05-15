using MetadataService.Models;

namespace MetadataService.Repositories;

public interface ITranscriptRepository : IRepository<Transcript>
{
    Task<IReadOnlyList<Transcript>> ListBySongAsync(Guid songId, CancellationToken ct = default);
}