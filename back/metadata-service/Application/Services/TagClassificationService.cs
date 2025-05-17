using AutoMapper;
using MetadataService.Domain.TagClassification;
using MetadataService.Infrastructure.Factories;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Application.Services;

internal sealed class TagClassificationService : ITagClassificationService
{
    private readonly ITagClassifierStrategy _classifier;
    private readonly ITagRepository         _tagRepo;
    private readonly IMapper                _mapper;

    public TagClassificationService(
        ITagClassifierFactory factory,
        ITagRepository        tagRepo,
        IMapper               mapper)
    {
        _classifier = factory.Create();
        _tagRepo    = tagRepo;
        _mapper     = mapper;
    }

    public async Task AttachTagsAsync(Song song, CancellationToken ct = default)
    {
        var lyrics = string.Join(' ', song.Transcripts.OrderBy(t => t.SegmentIndex).Select(t => t.Text));
        var rawTags = await _classifier.ClassifyAsync(lyrics, ct);
        if (!rawTags.Any()) return;

        var tagLookup = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
        // cache existing tags
        foreach (var t in await _tagRepo.ListAsync(null, ct))
            tagLookup[t.Name] = t;

        foreach (var tagName in rawTags)
        {
            if (!tagLookup.TryGetValue(tagName, out var tag))
            {
                tag = new Tag { Name = tagName, Approved = false };
                _tagRepo.Add(tag);
                tagLookup[tagName] = tag;
            }
            if (song.SongTags.All(st => st.TagId != tag.Id))
                song.SongTags.Add(new SongTag { Song = song, Tag = tag });
        }
    }
}