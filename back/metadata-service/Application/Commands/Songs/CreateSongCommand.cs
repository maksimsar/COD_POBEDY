using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record CreateSongCommand(CreateSongRequest Request) : IRequest<Guid>;