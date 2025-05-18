using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Commands;

public sealed record UpdateTranscriptTextCommand(long Id, UpdateTranscriptRequest Request) : IRequest<Unit>;