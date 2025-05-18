using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Songs;
using MetadataService.DTOs;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class ListSongsHandler 
    : IRequestHandler<ListSongsQuery, IReadOnlyList<SongDto>>
{
    private readonly ISongRepository _repo;
    private readonly IMapper _mapper;

    public ListSongsHandler(ISongRepository repo, IMapper mapper) =>
        (_repo, _mapper) = (repo, mapper);

    public async Task<IReadOnlyList<SongDto>> Handle(
        ListSongsQuery q, CancellationToken ct)
    {
        Expression<Func<Song, bool>> predicate = s =>
            (!q.AuthorId.HasValue  || s.SongAuthors.Any(sa => sa.AuthorId == q.AuthorId.Value))
            && (!q.TagId.HasValue     || s.SongTags   .Any(st => st.TagId    == q.TagId.Value))
            && (!q.Year.HasValue      || s.Year       == q.Year.Value);
        
        var songs = await _repo.ListAsync(predicate, ct);
        
        return songs
            .Select(s => _mapper.Map<SongDto>(s))
            .ToList();
    }
}