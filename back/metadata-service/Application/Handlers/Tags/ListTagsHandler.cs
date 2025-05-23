using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Tags;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class ListTagsHandler : IRequestHandler<ListTagsQuery, IReadOnlyList<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public ListTagsHandler(
        ITagRepository tagRepository,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }
    
    public async Task<IReadOnlyList<TagDto>> Handle(
        ListTagsQuery q, CancellationToken ct)
    {
        var tags = await _tagRepository.ListAsync(null, ct);      // null => без условия WHERE
        return tags.Select(t => _mapper.Map<TagDto>(t)).ToList();
    }
}