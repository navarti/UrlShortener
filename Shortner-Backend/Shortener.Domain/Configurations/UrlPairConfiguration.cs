using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortener.Domain.Entities;

namespace Shortener.Domain.Configurations;

public class UrlPairConfiguration : IEntityTypeConfiguration<UrlPair>
{
    public void Configure(EntityTypeBuilder<UrlPair> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ShortUrl).IsUnique();
    }
}
