using MediatR;

namespace MetadataService.Application.Commands;

public sealed record ApproveTagCommand(int Id) : IRequest<Unit>;