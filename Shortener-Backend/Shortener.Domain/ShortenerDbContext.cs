using Microsoft.EntityFrameworkCore;
using Shortener.Domain.Configurations;
using Shortener.Domain.Entities;

namespace Shortener.Domain;

public class ShortenerDbContext : DbContext
{
    public ShortenerDbContext(DbContextOptions<ShortenerDbContext> options) : base(options)
    {
    }

    public DbSet<UrlPair> UrlPairs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UrlPairConfiguration());
    }
}
