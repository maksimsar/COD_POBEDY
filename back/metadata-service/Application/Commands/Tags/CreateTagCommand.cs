using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record CreateTagCommand(CreateTagRequest Request) : IRequest<int>;