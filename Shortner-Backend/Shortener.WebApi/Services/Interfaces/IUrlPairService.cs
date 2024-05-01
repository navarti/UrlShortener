using Shortener.WebApi.Dtos;
using Shortener.WebApi.Util.Filters;

namespace Shortener.WebApi.Services.Interfaces;

public interface IUrlPairService
{
    Task<UrlPairDto> Create(CreateUrlPairDto urlPairCreateDTO);
    Task<UrlPairDto> GetById(Guid id);
    Task<IEnumerable<UrlPairDto>> GetAll(UrlPairFilter filter);
    Task<UrlPairDto> Update(UrlPairDto urlPairCreateDTO);
    Task Delete(Guid id);
}
