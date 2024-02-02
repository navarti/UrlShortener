namespace Shortener.WebApi.DTOs;

public class URLPairDTO
{
    public Guid Id { get; set; }
    public string LongUrl { get; set; } = "";
    public string ShortUrl { get; set; } = "";
    public DateTime CreationDate { get; set; }
    public long ClickedPerMonth { get; set; }
}
