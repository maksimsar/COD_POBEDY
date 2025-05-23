using MediatR;
using MetadataService.DTOs;

namespace MetadataService.Application.Queries.Transcripts;

public sealed record ListTranscriptsQuery(Guid SongId) : IRequest<IReadOnlyList<TranscriptDto>>;