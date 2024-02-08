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

    public async Task<UrlPairDTO> Create(UrlPairDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var urlPair = mapper.Map<UrlPair>(dto);
        urlPair.Id = default;

        var createdPair = await urlPairRepository.Create(urlPair).ConfigureAwait(false);
        if (createdPair is null)
        {
            throw new ArgumentException($"URL Pair creation failed");
        }

        return dto;
    }

    public async Task<UrlPairDTO> GetById(Guid id)
    {
        var urlPair = await urlPairRepository.GetById(id).ConfigureAwait(false);

        if (urlPair is null)
        {
            throw new ArgumentException(
                nameof(id),
                paramName: $"There are no recors in URLPairs table with such id - {id}.");
        }

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

        
        return urlPairs.Select(urlPair => mapper.Map<UrlPairDTO>(urlPair));
    }

    public async Task<UrlPairDTO> Update(UrlPairDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            var urlPair = await urlPairRepository.GetById(dto.Id);

            if (urlPair is null)
            {
                throw new ArgumentException("No URL pair with id, given in dto was not found");
            }

            mapper.Map(dto, urlPair);
            await urlPairRepository.Update(urlPair);
            return dto;
        }
        catch (DbUpdateException)
        {
            throw;
        }
    }

    public async Task Delete(Guid id)
    {
        var entity = new UrlPair() { Id = id };
        try
        {
            await urlPairRepository.Delete(entity).ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
    }
}
