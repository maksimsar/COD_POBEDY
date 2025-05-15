using AutoMapper;
using MetadataService.DTOs;
using MetadataService.Models;

namespace MetadataService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Domain -> DTO
        CreateMap<Tag, TagDto>();
        CreateMap<Author, AuthorDto>();

        CreateMap<Transcript, TranscriptDto>();

        CreateMap<Song, SongDto>()
            .ForMember(d => d.Tags,
                cfg => cfg.MapFrom(
                    s => s.SongTags.Select(st => st.Tag)))
            .ForMember(d => d.Authors,
                cfg => cfg.MapFrom(
                    s => s.SongAuthors.Select(sa => sa.Author)))
            .ForMember(d => d.Transcripts,
                cfg => cfg.MapFrom(s => s.Transcripts));

        // DTO -> Domain (для команд, где нужно)
        CreateMap<CreateSongRequest, Song>();
        CreateMap<UpdateSongRequest, Song>();
    }
}