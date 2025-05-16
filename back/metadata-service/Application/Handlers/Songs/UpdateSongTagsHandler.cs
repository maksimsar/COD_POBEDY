using MediatR;
using MassTransit;
using MetadataService.Application.Commands;
using MetadataService.Repositories;
using MetadataService.Messaging.Contracts;
using MetadataService.Models;

namespace MetadataService.Application.Handlers.Songs;

public sealed class UpdateSongTagsHandler : IRequestHandler<UpdateSongTagsCommand>
{
    private readonly ISongRepository _songRepo;
    private readonly ITagRepository  _tagRepo;
    private readonly IUnitOfWork     _uow;
    private readonly IPublishEndpoint _bus;

    public UpdateSongTagsHandler(ISongRepository songRepo, ITagRepository tagRepo,
        IUnitOfWork uow, IPublishEndpoint bus)
    {
        _songRepo = songRepo; _tagRepo = tagRepo; _uow = uow; _bus = bus;
    }

    public async Task Handle(UpdateSongTagsCommand cmd, CancellationToken ct)
    {
        var song = await _songRepo.GetFullAsync(cmd.Request.SongId, ct)
                   ?? throw new KeyNotFoundException("Song not found");

        song.SongTags.Clear();
        foreach (var tagId in cmd.Request.TagIds.Distinct())
        {
            var tag = await _tagRepo.GetByIdAsync(tagId, ct)
                      ?? throw new KeyNotFoundException($"Tag {tagId} not found");
            song.SongTags.Add(new SongTag { SongId = song.Id, TagId = tag.Id });
        }

        await _uow.SaveChangesAsync(ct);
        await _bus.Publish(new SongUpdatedV1(song.Id), ct);
    }
}