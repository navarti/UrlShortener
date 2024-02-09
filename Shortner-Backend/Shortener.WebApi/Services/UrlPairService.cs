using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;
using Shortener.WebApi.DTOs;
using Shortener.WebApi.Services.Interfaces;
using Shortener.WebApi.Util.Filters;

namespace Shortener.WebApi.Services;

public class UrlPairService : IUrlPairService
{
    private readonly IUrlPairRepository urlPairRepository;
    private readonly IMapper mapper;

    public UrlPairService(IUrlPairRepository urlPairRepository, IMapper mapper)
    {
        this.urlPairRepository = urlPairRepository ;
        this.mapper = mapper;
    }

    public async Task<UrlPairDTO> Create(CreateUrlPairDTO dto)
    {
        _ = dto ?? throw new ArgumentException("DTO is null");

        var urlPair = mapper.Map<UrlPair>(dto);
        urlPair.Id = default;

        var createdPair = await urlPairRepository.Create(urlPair).ConfigureAwait(false);
        _ = createdPair ?? throw new ArgumentException("URL Pair creation failed");

        return mapper.Map<UrlPairDTO>(createdPair);
    }

    public async Task<UrlPairDTO> GetById(Guid id)
    {
        var urlPair = await urlPairRepository.GetById(id).ConfigureAwait(false);

        _ = urlPair ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such id - {id}.");

        return mapper.Map<UrlPairDTO>(urlPair);
    }

    public async Task<IEnumerable<UrlPairDTO>> GetAll(UrlPairFilter filter)
    {
        filter ??= new UrlPairFilter();

        var urlPairs = await urlPairRepository
            .Get(
                skip: filter.Skip,
                take: filter.Take,
                whereExpression: filter.WhereExpression,
                orderBy: filter.OrderBy,
                asNoTracking: filter.AsNoTracking)
            .ToListAsync()
            .ConfigureAwait(false);

        if (!urlPairs.Any())
        {
            throw new ArgumentException($"There are no records with such filter.");
        }

        return urlPairs.Select(urlPair => mapper.Map<UrlPairDTO>(urlPair));
    }

    public async Task<UrlPairDTO> Update(UrlPairDTO dto)
    {
        _ = dto ?? throw new ArgumentException("DTO is null");

        var urlPair = await urlPairRepository.GetById(dto.Id);

        _ = urlPair ?? throw new ArgumentException("No URL pair with id, given in dto was not found");

        mapper.Map(dto, urlPair);
        await urlPairRepository.Update(urlPair);
        return dto;
    }

    public async Task Delete(Guid id)
    {
        var entity = await urlPairRepository.GetById(id).ConfigureAwait(false);
        _ = entity ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such dto - {id}.");
        await urlPairRepository.Delete(entity).ConfigureAwait(false);
    }
}
