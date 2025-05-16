using MediatR;
using AutoMapper;
using MassTransit;
using MetadataService.Application.Commands;
using MetadataService.DTOs;
using MetadataService.Models;
using MetadataService.Repositories;
using MetadataService.Domain.Builders;
using Common.Events;

public sealed class CreateSongHandler : IRequestHandler<CreateSongCommand, Guid>
{
    private readonly ISongRepository   _songRepo;
    private readonly IAuthorRepository _authorRepo;
    private readonly ITagRepository    _tagRepo;
    private readonly IUnitOfWork       _uow;
    private readonly IMapper           _mapper;
    private readonly IPublishEndpoint  _bus;
    private readonly ISongBuilder       _builder;

    public CreateSongHandler(
        ISongRepository   songRepo,
        IAuthorRepository authorRepo,
        ITagRepository    tagRepo,
        IUnitOfWork       uow,
        IMapper           mapper,
        IPublishEndpoint  bus,
        ISongBuilder       builder)
    {
        _songRepo  = songRepo;
        _authorRepo= authorRepo;
        _tagRepo   = tagRepo;
        _uow       = uow;
        _mapper    = mapper;
        _bus       = bus;
        _builder   = builder;
    }

    public async Task<Guid> Handle(CreateSongCommand cmd, CancellationToken ct)
    {
        // 1. Маппинг и сборка
        var song = _mapper.Map<Song>(cmd.Request);
        
        var builderWithAuthors = await _builder
            .For(song)
            .AttachAuthorsAsync(cmd.Request.AuthorIds.ToArray(), ct);
        
        var builderWithTags = await builderWithAuthors
            .AttachTagsAsync(cmd.Request.TagIds.ToArray(), ct);
        
        var builtSong = await builderWithTags
            .BuildAsync(ct);

        // 2. Сохранение
        _songRepo.Add(song);
        await _uow.SaveChangesAsync(ct);

        // 3. Событие
        await _bus.Publish(new SongUpdatedV1(song.Id), ct);

        return song.Id;
    }
}