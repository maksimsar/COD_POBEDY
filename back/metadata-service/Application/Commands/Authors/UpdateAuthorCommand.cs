using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record UpdateAuthorCommand(Guid Id, UpdateAuthorRequest Request) : IRequest<Unit>;