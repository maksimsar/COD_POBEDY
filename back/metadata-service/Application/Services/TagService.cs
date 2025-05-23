using AutoMapper;
using MetadataService.DTOs;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Services;

internal sealed class TagService : ITagService
{
    private readonly ITagRepository _tagRepo;
    private readonly ISongRepository _songRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(
        ITagRepository tagRepo,
        ISongRepository songRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _tagRepo = tagRepo;
        _songRepo = songRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TagDto> GetAsync(int id, CancellationToken ct = default)
    {
        var tag = await _tagRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Tag with id {id} not found");
        return _mapper.Map<TagDto>(tag);
    }

    public async Task<IReadOnlyList<TagDto>> ListAsync(CancellationToken ct = default)
    {
        var tags = await _tagRepo.ListAsync(null, ct);
        return _mapper.Map<IReadOnlyList<TagDto>>(tags);
    }

    public async Task<int> CreateAsync(CreateTagRequest request, CancellationToken ct = default)
    {
        if(await _tagRepo.GetByNameAsync(request.Name, ct) != null)
            throw new InvalidOperationException($"Tag with name {request.Name} already exists");
        var tag = _mapper.Map<Tag>(request);
        _tagRepo.Add(tag);
        await _unitOfWork.SaveChangesAsync(ct);
        return tag.Id;
    }

    public async Task ApproveAsync(int id, CancellationToken ct = default)
    {
        var tag = await _tagRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Tag with id {id} not found");
        if(tag.Approved) return;
        
        tag.Approved = true;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(int id,UpdateTagRequest request, CancellationToken ct = default)
    {
        var tag = await _tagRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Tag with id {id} not found");
        
        if(request.Name is not null && tag.Name != request.Name && await _tagRepo.GetByNameAsync(request.Name, ct) != null )
            throw new InvalidOperationException($"Tag with name {request.Name} already exists");
        _mapper.Map(request, tag);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var tag = await _tagRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Tag with id {id} not found");
        _tagRepo.Remove(tag);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}