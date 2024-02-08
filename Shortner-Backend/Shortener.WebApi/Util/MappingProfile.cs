using AutoMapper;
using Shortener.Domain.Entities;
using Shortener.WebApi.DTOs;

namespace Shortener.WebApi.Util;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UrlPair, UrlPairDTO>();
    }
}
