using MetadataService.Models;

namespace MetadataService.Application.Services;

public interface ITagClassificationService
{
    Task AttachTagsAsync(Song song, CancellationToken ct = default);
}