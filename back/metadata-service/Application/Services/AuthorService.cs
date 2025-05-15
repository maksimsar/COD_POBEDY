using AutoMapper;
using MetadataService.DTOs;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Services;

internal sealed class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepo;
    private readonly ISongRepository _songRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthorService(
        IAuthorRepository authorRepo,
        ISongRepository songRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _authorRepo = authorRepo;
        _songRepo = songRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AuthorDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByIdAsync(id, ct);
        return author is null ? null : _mapper.Map<AuthorDto?>(author);
    }

    public async Task<IReadOnlyList<AuthorDto>> ListAsync(CancellationToken ct = default)
    {
        var authors = await _authorRepo.ListAsync(null, ct);
        return _mapper.Map<IReadOnlyList<AuthorDto>>(authors);
    }

    public async Task<Guid> CreateAsync(CreateAuthorRequest request, CancellationToken ct = default)
    {
        if(await _authorRepo.ExistsAsync(request.FullName,ct))
            throw new InvalidOperationException($"Author {request.FullName} already exists");
        
        var author = _mapper.Map<Author>(request);
        _authorRepo.Add(author);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return author.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateAuthorRequest request, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Author {id} not found");
        
        _mapper.Map(request, author);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Author {id} not found");
        _authorRepo.Remove(author);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}