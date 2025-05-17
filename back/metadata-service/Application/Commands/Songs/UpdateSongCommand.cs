using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record UpdateSongCommand(Guid SongId, UpdateSongRequest Request) : IRequest;