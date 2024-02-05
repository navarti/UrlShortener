using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;

namespace Shortener.Domain.Repositories;

public class UrlPairRepository : EntityRepositoryBase<Guid, UrlPair>, IUrlPairRepository
{
    public UrlPairRepository(ShortenerDbContext dbContext) : base(dbContext)
    {
    }
}
