using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record UpdateTagCommand(int Id, UpdateTagRequest Request) : IRequest<Unit>;