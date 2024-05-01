using AutoMapper;
using Shortener.Domain.Entities;
using Shortener.WebApi.Dtos;

namespace Shortener.WebApi.Util;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UrlPair, UrlPairDto>().ReverseMap();
        CreateMap<CreateUrlPairDto, UrlPair>();
    }
}
