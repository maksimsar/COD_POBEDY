using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Songs;

public sealed record GetSongQuery(Guid Id) : IRequest<SongDto?>;