using MediatR;

namespace MetadataService.Application.Commands;

public sealed record DeleteTranscriptCommand(long Id) : IRequest<Unit>;