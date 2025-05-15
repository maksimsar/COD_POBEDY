using AutoMapper;
using MassTransit;
using MetadataService.DTOs;
using MetadataService.Domain.Builders;
using MetadataService.Models;
using MetadataService.Messaging.Contracts;
using MetadataService.Repositories;

namespace MetadataService.Application.Services;

internal sealed class SongService : ISongService
{
    private readonly ISongRepository     _songRepo;
    private readonly IAuthorRepository   _authorRepo;
    private readonly ITagRepository      _tagRepo;
    private readonly IUnitOfWork         _uow;
    private readonly IMapper             _mapper;
    private readonly IPublishEndpoint    _bus;
    private readonly SongBuilder         _builder; // паттерн Builder

    public SongService(
        ISongRepository   songRepo,
        IAuthorRepository authorRepo,
        ITagRepository    tagRepo,
        IUnitOfWork       uow,
        IMapper           mapper,
        IPublishEndpoint  bus,
        SongBuilder       builder)
    {
        _songRepo  = songRepo;
        _authorRepo= authorRepo;
        _tagRepo   = tagRepo;
        _uow       = uow;
        _mapper    = mapper;
        _bus       = bus;
        _builder   = builder;
    }
    

    public async Task<SongDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var song = await _songRepo.GetFullAsync(id, ct);
        return song is null ? null : _mapper.Map<SongDto>(song);
    }

    public async Task<IReadOnlyList<SongDto>> ListAsync(
        short? year = null, Guid? authorId = null, int? tagId = null, CancellationToken ct = default)
    {
        // Генерируем предикат на лету
        var songs = await _songRepo.ListAsync(s =>
            (year      == null || s.Year == year) &&
            (authorId  == null || s.SongAuthors.Any(a => a.AuthorId == authorId)) &&
            (tagId     == null || s.SongTags.Any(t => t.TagId == tagId)), ct);

        return _mapper.Map<IReadOnlyList<SongDto>>(songs);
    }
    

    public async Task<Guid> CreateAsync(CreateSongRequest req, CancellationToken ct = default)
    {
        // 1) маппим базовые поля
        var song = _mapper.Map<Song>(req);

        // 2) через Builder добавляем авторов и теги
        await _builder
            .For(song)
            .AttachAuthorsAsync(req.AuthorIds, _authorRepo, ct)
            .AttachTagsAsync(req.TagIds, _tagRepo, ct)
            .BuildAsync();

        _songRepo.Add(song);
        await _uow.SaveChangesAsync(ct);

        await _bus.Publish(new SongUpdatedV1(song.Id), ct);     // сообщение для search-сервиса
        return song.Id;
    }
    

    public async Task UpdateAsync(Guid id, UpdateSongRequest req, CancellationToken ct = default)
    {
        var song = await _songRepo.GetByIdAsync(id, ct) 
                   ?? throw new KeyNotFoundException("Song not found");

        _mapper.Map(req, song);            // патч-копируем поля
        await _uow.SaveChangesAsync(ct);
        await _bus.Publish(new SongUpdatedV1(song.Id), ct);
    }

    public async Task UpdateTagsAsync(UpdateTagsRequest req, CancellationToken ct = default)
    {
        var song = await _songRepo.GetFullAsync(req.SongId, ct) 
                   ?? throw new KeyNotFoundException("Song not found");
        
        song.SongTags.Clear();

        // привязываем новые
        foreach (var tagId in req.TagIds.Distinct())
        {
            var tag = await _tagRepo.GetByIdAsync(tagId, ct) 
                      ?? throw new KeyNotFoundException($"Tag {tagId} not found");
            song.SongTags.Add(new SongTag { SongId = song.Id, TagId = tag.Id });
        }

        await _uow.SaveChangesAsync(ct);
        await _bus.Publish(new SongUpdatedV1(song.Id), ct);
    }
    

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var song = await _songRepo.GetByIdAsync(id, ct) 
                   ?? throw new KeyNotFoundException("Song not found");
        _songRepo.Remove(song);
        await _uow.SaveChangesAsync(ct);
    }
}