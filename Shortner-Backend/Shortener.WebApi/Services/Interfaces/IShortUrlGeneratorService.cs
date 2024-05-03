namespace Shortener.WebApi.Services.Interfaces;

public interface IShortUrlGeneratorService
{
    class GenerateShortUrlResult
    {
        public string ShortUrl { get; set; } = "";
        public bool IsNew { get; set; } = false;
    }

    public GenerateShortUrlResult GenerateShortUrl();
    public void UpdateFreeUrls();
}
