using MediatR;

namespace MetadataService.Application.Commands;

public sealed record ApproveTranscriptCommand(long Id, Guid? CheckedById = null) : IRequest<Unit>;