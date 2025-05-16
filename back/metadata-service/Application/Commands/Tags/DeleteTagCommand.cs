using MediatR;

namespace MetadataService.Application.Commands;

public sealed record DeleteTagCommand(int Id) : IRequest<Unit>;