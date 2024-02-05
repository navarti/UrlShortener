using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;
using Shortener.WebApi.DTOs;
using Shortener.WebApi.Services.Interfaces;

namespace Shortener.WebApi.Services;

public class UrlPairService : IUrlPairService
{
    private readonly IUrlPairRepository urlPairRepository;
    private readonly IUrlPairService urlPairServive;
    private readonly IMapper mapper;

    public UrlPairService(IUrlPairRepository urlPairRepository, IUrlPairService urlPairServive, IMapper mapper)
    {
        this.urlPairRepository = urlPairRepository ?? throw new ArgumentNullException(nameof(urlPairRepository));
        this.urlPairServive = urlPairServive ?? throw new ArgumentNullException(nameof(urlPairServive));
        this.mapper = mapper;
    }

    public async Task<UrlPairDTO> Create(UrlPairDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var urlPair = mapper.Map<UrlPair>(dto);
        urlPair.Id = default;

        var newTeacher = await urlPairRepository.Create(urlPair).ConfigureAwait(false);

        return dto;
    }

    public async Task<UrlPairDTO> GetById(Guid id)
    {
        var urlPair = await urlPairRepository.GetById(id).ConfigureAwait(false);

        if (urlPair == null)
        {
            throw new ArgumentException(
                nameof(id),
                paramName: $"There are no recors in URLPairs table with such id - {id}.");
        }

        return mapper.Map<UrlPairDTO>(urlPair);
    }

    public async Task<IEnumerable<UrlPairDTO>> GetAll()
    {
        var urlPairs = await urlPairRepository.GetAll().ConfigureAwait(false);
        
        return urlPairs.Select(urlPair => mapper.Map<UrlPairDTO>(urlPair)).ToList();
    }

    public async Task<UrlPairDTO> Update(UrlPairDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return await ExecuteUpdate(dto).ConfigureAwait(false);
    }

    private async Task<UrlPairDTO> ExecuteUpdate(UrlPairDTO dto)
    {
        try
        {
            var urlPair = await urlPairRepository.GetById(dto.Id);

            if (urlPair is null)
            {
                throw new ArgumentException("No URL pair with id, given in dto was not found");
            }

            mapper.Map(dto, urlPair);
            return dto;
        }
        catch (DbUpdateException ex)
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
