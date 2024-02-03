using Shortener.WebApi.DTOs;

namespace Shortener.WebApi.Services.Interfaces;

public interface IUrlPairService
{
    Task<UrlPairDTO> Create(UrlPairDTO urlPairCreateDTO);
    Task<UrlPairDTO> Get(Guid id);
    Task<IEnumerable<UrlPairDTO>> GetAll();
    Task<UrlPairDTO> Update(UrlPairDTO urlPairCreateDTO);
    Task Delete(Guid id);
}
