using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Songs;

public sealed record ListSongsQuery(
    Guid? AuthorId,
    int? TagId,
    int? Year) : IRequest<IReadOnlyList<SongDto>>;