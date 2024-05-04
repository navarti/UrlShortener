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

        Uri? uriResult;
        bool result = Uri.TryCreate(dto.LongUrl, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        if (!result)
            throw new ArgumentException("URL is incorrect");

        var existingUrlPair = await urlPairRepository.GetFirstOrDefaultAsync(u => u.LongUrl == dto.LongUrl && !u.IsDeleted)
            .ConfigureAwait(false);

        if(existingUrlPair != null)
        {
            // Maybe throw an exception here in future?
            return mapper.Map<UrlPairDto>(existingUrlPair);
        }
        
        var generatedShortUrl = shortUrlGeneratorService.GenerateShortUrl();

        if (generatedShortUrl.IsNew)
        {
            var urlPair = mapper.Map<UrlPair>(dto);

            urlPair.Id = default;
            urlPair.ShortUrl = generatedShortUrl.ShortUrl;
            urlPair.CreationDate = DateTime.UtcNow;
            urlPair.ClickedPerMonth = 0;
            urlPair.IsDeleted = false;

            var createdPair = await urlPairRepository.Create(urlPair).ConfigureAwait(false);

            _ = createdPair ?? throw new ArgumentException("URL Pair creation failed");

            return mapper.Map<UrlPairDto>(createdPair);
        }
        else
        {
            var urlPair = await urlPairRepository.GetByShortUrlAsync(generatedShortUrl.ShortUrl).ConfigureAwait(false);
            
            _ = urlPair ?? throw new ArgumentException("URL Pair creation failed");
            urlPair.LongUrl = dto.LongUrl;
            urlPair.CreationDate = DateTime.UtcNow;
            urlPair.ClickedPerMonth = 0;
            urlPair.IsDeleted = false;
            
            await urlPairRepository.Update(urlPair);
            return mapper.Map<UrlPairDto>(urlPair);
        }
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

        if(!urlPairs.Any())
        {
            return Enumerable.Empty<UrlPairDto>();
        }

        return urlPairs.Select(urlPair => mapper.Map<UrlPairDto>(urlPair));
    }

    public async Task<UrlPairDto> Update(UrlPairDto dto)
    {
        _ = dto ?? throw new ArgumentException("DTO is null");

        var urlPair = await urlPairRepository.GetById(dto.Id);

        _ = urlPair ?? throw new ArgumentException("No URL pair with id, given in dto was not found");

        var existingUrlPair = await urlPairRepository.GetByShortUrlAsync(dto.ShortUrl).ConfigureAwait(false);

        if(existingUrlPair != null && existingUrlPair.Id != dto.Id)
        {
            throw new ArgumentException("This short URL is already in use");
        }

        mapper.Map(dto, urlPair);

        await urlPairRepository.Update(urlPair);

        return dto;
    }

    public async Task Delete(Guid id)
    {
        var entity = await urlPairRepository.GetById(id).ConfigureAwait(false);

        _ = entity ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such id - {id}.");

        await urlPairRepository.Delete(entity).ConfigureAwait(false);
    }

    public async Task SoftDelete(Guid id)
    {
        var entity = await urlPairRepository.GetById(id).ConfigureAwait(false);

        _ = entity ?? throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such id - {id}.");

        if(entity.IsDeleted)
            throw new ArgumentException(
                nameof(id),
                paramName: $"There are no records in URLPairs table with such id - {id}.");

        entity.IsDeleted = true;
        await urlPairRepository.Update(entity);
    }

    public async Task<string> GetLongUrlByShort(string shortUrl)
    {
        // IsDeleted ignored by default
        var entity = await urlPairRepository.GetByShortUrlAsync(shortUrl).ConfigureAwait(false);

        _ = entity ?? throw new ArgumentException(
                shortUrl,
                paramName: $"There are no records in URLPairs table with such shortUrl - {shortUrl}.");

        return entity.LongUrl;
    }
}
