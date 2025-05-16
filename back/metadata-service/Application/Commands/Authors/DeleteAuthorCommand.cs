using MediatR;

namespace MetadataService.Application.Commands;

public sealed record DeleteAuthorCommand(Guid Id) : IRequest<Unit>;