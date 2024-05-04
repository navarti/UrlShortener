using Microsoft.EntityFrameworkCore;
using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;

namespace Shortener.Domain.Repositories.Realizations;

public class UrlPairRepository : EntityRepositoryBase<Guid, UrlPair>, IUrlPairRepository
{
    public UrlPairRepository(ShortenerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<UrlPair?> GetByShortUrlAsync(string shortUrl) => await dbSet.FirstOrDefaultAsync(x => x.ShortUrl == shortUrl);
}
