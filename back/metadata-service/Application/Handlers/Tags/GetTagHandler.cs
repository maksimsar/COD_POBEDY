using AutoMapper;
using MediatR;
using MetadataService.Application.Queries.Tags;
using MetadataService.DTOs;
using MetadataService.Repositories;

namespace MetadataService.Application.Handlers;

internal sealed class GetTagHandler : IRequestHandler<GetTagQuery, TagDto?>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetTagHandler(
        ITagRepository tagRepository,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }
    
    public async Task<TagDto?> Handle(GetTagQuery q, CancellationToken ct)
    {
        var tag = await _tagRepository.GetByIdAsync(q.Id, ct);
        return tag is null ? null : _mapper.Map<TagDto>(tag);
    }
}