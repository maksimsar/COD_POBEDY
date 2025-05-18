using AutoMapper;
using MetadataService.DTOs;
using MetadataService.Models;

namespace MetadataService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Tag, TagDto>();

        CreateMap<Author, AuthorDto>();

        CreateMap<Transcript, TranscriptDto>();

        CreateMap<Song, SongDto>()
            .ForMember(d => d.Tags, cfg => cfg
                .MapFrom(s => s.SongTags.Select(st => st.Tag)))
            .ForMember(d => d.Authors, cfg => cfg
                .MapFrom(s => s.SongAuthors.Select(sa => sa.Author)))
            .ForMember(d => d.Transcripts, cfg => cfg
                .MapFrom(s => s.Transcripts));
        
        CreateMap<CreateSongRequest, Song>();
        CreateMap<UpdateSongRequest, Song>()
            .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember is not null));
        
        CreateMap<CreateAuthorRequest, Author>();
        CreateMap<UpdateAuthorRequest, Author>()
            .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember is not null));
        
        CreateMap<CreateTagRequest, Tag>();
        CreateMap<UpdateTagRequest, Tag>()
            .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember is not null));
        
        CreateMap<UpdateTranscriptRequest, Transcript>()
            .ForMember(d => d.Text, opt => opt.MapFrom(src => src.Text));
    }
}