using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Author;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class GetAuthorHandler : IRequestHandler<GetAuthorQuery, AuthorDto?>
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IMapper _mapper;

    public GetAuthorHandler(
        IAuthorRepository authorRepo,
        IMapper mapper)
    {
        _authorRepo = authorRepo;
        _mapper = mapper;
    }
    
    public async Task<AuthorDto?> Handle(GetAuthorQuery q, CancellationToken ct)
    {
        var author = await _authorRepo.GetByIdAsync(q.Id, ct);
        return author is null ? null : _mapper.Map<AuthorDto>(author);
    }
}