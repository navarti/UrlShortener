using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;
using Shortener.WebApi.Dtos;
using Shortener.WebApi.Services.Interfaces;
using Shortener.WebApi.Util.Filters;

namespace Shortener.WebApi.Services.Realizations;

public class UrlPairService : IUrlPairService
{
    private readonly IUrlPairRepository urlPairRepository;
    private readonly IMapper mapper;
    private readonly IShortUrlGeneratorService shortUrlGeneratorService;

    public UrlPairService(IUrlPairRepository urlPairRepository, IMapper mapper, IShortUrlGeneratorService shortUrlGeneratorService)
    {
        this.urlPairRepository = urlPairRepository;
        this.mapper = mapper;
        this.shortUrlGeneratorService = shortUrlGeneratorService;
    }

    public async Task<UrlPairDto> Create(CreateUrlPairDto dto)
    {
        _ = dto ?? throw new ArgumentException("DTO is null");

        var existingUrlPair = await urlPairRepository.Get(
            whereExpression: u => u.LongUrl == dto.LongUrl)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if(existingUrlPair != null)
        {
            // Maybe throw an exception here in future?
            return mapper.Map<UrlPairDto>(existingUrlPair);
        }
        
        var urlPair = mapper.Map<UrlPair>(dto);

        urlPair.Id = default;
        urlPair.ShortUrl = shortUrlGeneratorService.GenerateShortUrl();
        urlPair.CreationDate = DateTime.UtcNow;
        urlPair.ClickedPerMonth = 0;

        var createdPair = await urlPairRepository.Create(urlPair).ConfigureAwait(false);
        _ = createdPair ?? throw new ArgumentException("URL Pair creation failed");
        
        return mapper.Map<UrlPairDto>(createdPair);
    }

    public async Task<UrlPairDto> GetById(Guid id)
    {
        var urlPair = await urlPairRepository.GetById(id).ConfigureAwait(false);

        _ = urlPair ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such id - {id}.");

        return mapper.Map<UrlPairDto>(urlPair);
    }

    public async Task<IEnumerable<UrlPairDto>> GetAll(UrlPairFilter filter)
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

        return urlPairs.Select(urlPair => mapper.Map<UrlPairDto>(urlPair));
    }

    public async Task<UrlPairDto> Update(UrlPairDto dto)
    {
        _ = dto ?? throw new ArgumentException("DTO is null");

        var urlPair = await urlPairRepository.GetById(dto.Id);

        _ = urlPair ?? throw new ArgumentException("No URL pair with id, given in dto was not found");

        mapper.Map(dto, urlPair);

        await urlPairRepository.Update(urlPair);

        return dto;
    }

    public async Task SoftDelete(Guid id)
    {
        var entity = await urlPairRepository.GetById(id).ConfigureAwait(false);

        _ = entity ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such dto - {id}.");

        await urlPairRepository.Delete(entity).ConfigureAwait(false);
    }
}
