﻿using Shortener.Domain.Entities;

namespace Shortener.Domain.Repositories.Interfaces;

public interface IUrlPairRepository : IEntityRepository<Guid, UrlPair>
{
    public Task<UrlPair?> GetByShortUrlAsync(string shortUrl);
}
