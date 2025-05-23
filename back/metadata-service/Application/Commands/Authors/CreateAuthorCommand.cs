using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record CreateAuthorCommand(CreateAuthorRequest Request) : IRequest<Guid>;