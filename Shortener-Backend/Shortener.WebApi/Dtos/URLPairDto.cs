namespace Shortener.WebApi.Dtos;

public class UrlPairDto : CreateUrlPairDto
{
    public Guid Id { get; set; }
    public string ShortUrl { get; set; } = "";
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public long ClickedPerMonth { get; set; } = 0;
}
