using Shortener.Common.Enums;
using Shortener.Domain.Entities;
using Shortener.Domain.Repositories.Interfaces;
using Shortener.WebApi.Services.Interfaces;
using System.Linq.Expressions;
using System.Text;
using static Shortener.WebApi.Services.Interfaces.IShortUrlGeneratorService;

namespace Shortener.WebApi.Services.Realizations;

public class ShortUrlGeneratorService : IShortUrlGeneratorService
{
    private readonly IServiceProvider _serviceProvider;

    private string lastUrl = "";
    private readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

    private const int maxFreeUrls = 100;
    private List<string> freeUrls = new();

    public ShortUrlGeneratorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitLastUrl();
        UpdateFreeUrls();
    }

    private void InitLastUrl()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var urlPairRepository = scope.ServiceProvider.GetRequiredService<IUrlPairRepository>();

            var lastUrlsInDb = urlPairRepository.Get(
                orderBy: new Dictionary<Expression<Func<UrlPair, object>>, SortDirection>
                {
                    { u => u.CreationDate, SortDirection.Descending }
                }
                );
            
            if(lastUrlsInDb.Any())
            {
                lastUrl = lastUrlsInDb.First().ShortUrl;
            }
        }
    }

    public void UpdateFreeUrls()
    {
        freeUrls.Clear();

        using (var scope = _serviceProvider.CreateScope())
        {
            var urlPairRepository = scope.ServiceProvider.GetRequiredService<IUrlPairRepository>();

            freeUrls.AddRange(
                urlPairRepository.Get(
                    whereExpression: u => u.IsDeleted,
                    orderBy: new Dictionary<Expression<Func<UrlPair, object>>, SortDirection>
                    {
                        { u => u.ShortUrl.Length, SortDirection.Ascending }
                    },
                    take: maxFreeUrls)
                .Select(u => u.ShortUrl));
        }
    }

    public GenerateShortUrlResult GenerateShortUrl()
    {
        if (freeUrls.Count > 0)
        {
            string url = freeUrls[0];
            freeUrls.RemoveAt(0);

            if(freeUrls.Count == 0)
            {
                UpdateFreeUrls();
            }

            return new GenerateShortUrlResult { ShortUrl = url, IsNew = false };
        }

        StringBuilder result = new StringBuilder(lastUrl);

        for (int i = result.Length - 1; i >= 0; i--)
        {
            int charIndex = Array.IndexOf(chars, result[i]);

            // If the character is not '9', increment it and return
            if (charIndex < chars.Length - 1)
            {
                result[i] = chars[charIndex + 1];

                lastUrl = result.ToString();
                return new GenerateShortUrlResult { ShortUrl = lastUrl, IsNew = true };
            }
            // If the character is '9', reset it to 'A' and continue to the previous character
            else
            {
                result[i] = chars[0];
            }
        }

        // If all characters are '9', add a new character 'A' at the beginning
        result.Insert(0, chars[0]);

        lastUrl = result.ToString();
        return new GenerateShortUrlResult { ShortUrl = lastUrl, IsNew = true };
    }
}
