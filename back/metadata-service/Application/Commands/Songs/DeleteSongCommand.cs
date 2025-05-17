using MediatR;

namespace MetadataService.Application.Commands;

public sealed record DeleteSongCommand(Guid SongId) : IRequest;