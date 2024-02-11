using Shortener.WebApi.DTOs;
using Shortener.WebApi.Util.Filters;

namespace Shortener.WebApi.Services.Interfaces;

public interface IUrlPairService
{
    Task<UrlPairDTO> Create(CreateUrlPairDTO urlPairCreateDTO);
    Task<UrlPairDTO> GetById(Guid id);
    Task<IEnumerable<UrlPairDTO>> GetAll(UrlPairFilter filter);
    Task<UrlPairDTO> Update(UrlPairDTO urlPairCreateDTO);
    Task Delete(Guid id);
}
