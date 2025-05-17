using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Author;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class ListAuthorsHandler : IRequestHandler<ListAuthorsQuery, IReadOnlyList<AuthorDto>>
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IMapper _mapper;

    public ListAuthorsHandler(
        IAuthorRepository authorRepo,
        IMapper mapper)
    {
        _authorRepo = authorRepo;
        _mapper = mapper;
    }
    
    public async Task<IReadOnlyList<AuthorDto>> Handle(
        ListAuthorsQuery q, CancellationToken ct)
    {
        var authors = await _authorRepo.ListAsync(null, ct);   // без условия — все записи
        return authors.Select(a => _mapper.Map<AuthorDto>(a)).ToList();
    }
}