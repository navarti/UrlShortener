using Shortener.Domain.Entities.Interfaces;

namespace Shortener.Domain.Entities;

public class UrlPair : IKeyedEntity<Guid>
{
    public Guid Id { get; set; }

    public string LongUrl { get; set; } = "";

    public string ShortUrl { get; set; } = "";

    public DateTime CreationDate { get; set; }

    public  long ClickedPerMonth { get; set; }
}
