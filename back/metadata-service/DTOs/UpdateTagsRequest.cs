namespace MetadataService.DTOs;

public sealed record UpdateTagsRequest(Guid SongId, IReadOnlyList<int> TagIds);