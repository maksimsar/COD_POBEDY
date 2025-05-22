using AutoMapper;
using TranscriptionService.DTOs;
using TranscriptionService.Models;

namespace TranscriptionService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TranscriptionJob, TranscriptionJobDto>();
    }
}