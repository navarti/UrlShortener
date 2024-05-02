namespace Shortener.WebApi.Services.Interfaces;

public interface IShortUrlGeneratorService
{
    public string GenerateShortUrl();
    public void UpdateFreeUrls();
}
