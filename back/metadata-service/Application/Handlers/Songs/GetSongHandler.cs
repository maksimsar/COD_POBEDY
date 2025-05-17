using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Songs;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class GetSongHandler 
    : IRequestHandler<GetSongQuery, SongDto?>
{
    private readonly ISongRepository _repo;
    private readonly IMapper         _mapper;

    public GetSongHandler(ISongRepository repo, IMapper mapper) =>
        (_repo, _mapper) = (repo, mapper);

    public async Task<SongDto?> Handle(GetSongQuery q, CancellationToken ct)
    {
        var song = await _repo.GetFullAsync(q.Id, ct);
        return song is null ? null : _mapper.Map<SongDto>(song);
    }
}