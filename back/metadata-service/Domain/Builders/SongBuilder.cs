using AutoMapper;
using MetadataService.Models;
using MetadataService.Repositories;

namespace MetadataService.Domain.Builders;

internal sealed class SongBuilder : ISongBuilder
{
    private readonly IAuthorRepository     _authors;
    private readonly ITagRepository        _tags;
    private readonly ITranscriptRepository _transcripts;
    private readonly IMapper               _mapper;

    private Song? _song;

    public SongBuilder(
        IAuthorRepository     authors,
        ITagRepository        tags,
        ITranscriptRepository transcripts,
        IMapper               mapper)
    {
        _authors     = authors;
        _tags        = tags;
        _transcripts = transcripts;
        _mapper      = mapper;
    }

    public ISongBuilder For(Song song)
    {
        if (_song is not null)
            throw new InvalidOperationException("SongBuilder already initialized – create new instance.");
        _song = song ?? throw new ArgumentNullException(nameof(song));
        return this;
    }

    public async Task<ISongBuilder> AttachAuthorsAsync(Guid[] authorIds, CancellationToken ct = default)
    {
        EnsureSong();
        if (authorIds is null || authorIds.Length == 0)
            throw new ArgumentException("authorIds cannot be empty");

        var set = new HashSet<Guid>(_song!.SongAuthors.Select(sa => sa.AuthorId));
        foreach (var id in authorIds.Distinct())
        {
            if (set.Contains(id)) continue; // уже привязан
            var author = await _authors.GetByIdAsync(id, ct)
                         ?? throw new KeyNotFoundException($"Author {id} not found");
            _song.SongAuthors.Add(new SongAuthor { Song = _song, Author = author });
            set.Add(id);
        }
        return this;
    }

    public async Task<ISongBuilder> AttachTagsAsync(int[] tagIds, CancellationToken ct = default)
    {
        EnsureSong();
        if (tagIds is null || tagIds.Length == 0)
            throw new ArgumentException("tagIds cannot be empty");

        var set = new HashSet<int>(_song!.SongTags.Select(st => st.TagId));
        foreach (var id in tagIds.Distinct())
        {
            if (set.Contains(id)) continue;
            var tag = await _tags.GetByIdAsync(id, ct)
                      ?? throw new KeyNotFoundException($"Tag {id} not found");
            _song.SongTags.Add(new SongTag { Song = _song, Tag = tag });
            set.Add(id);
        }
        return this;
    }

    public ISongBuilder AddTranscriptSegment(Transcript segment)
    {
        EnsureSong();
        if (segment is null) throw new ArgumentNullException(nameof(segment));
        segment.Song = _song!;
        _song!.Transcripts.Add(segment);
        return this;
    }

    public Task<Song> BuildAsync(CancellationToken ct = default)
    {
        EnsureSong();

        if (_song!.SongAuthors.Count == 0)
            throw new InvalidOperationException("Song must have at least one author");
        if (_song.SongTags.Count == 0)
            throw new InvalidOperationException("Song must have at least one tag");

        var result = _song;
        _song = null;                      // защита от повторного использования
        return Task.FromResult(result);
    }

    private void EnsureSong()
    {
        if (_song is null)
            throw new InvalidOperationException("Call For(song) first");
    }
}
