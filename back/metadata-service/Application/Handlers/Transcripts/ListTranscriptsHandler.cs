using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Transcripts;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class ListTranscriptsHandler : IRequestHandler<ListTranscriptsQuery, IReadOnlyList<TranscriptDto>>
{
    private readonly ITranscriptRepository _transcriptRepo;
    private readonly IMapper _mapper;

    public ListTranscriptsHandler(
        ITranscriptRepository transcriptRepo,
        IMapper mapper)
    {
        _transcriptRepo = transcriptRepo;
        _mapper = mapper;
    }
    
    public async Task<IReadOnlyList<TranscriptDto>> Handle(
        ListTranscriptsQuery q, CancellationToken ct)
    {
        var segments = await _transcriptRepo.ListBySongAsync(q.SongId, ct);
        return segments.Select(s => _mapper.Map<TranscriptDto>(s)).ToList();
    }
}