using UrlShortener.Core.Interfaces;

namespace UrlShortener.Core.Services;

public class UrlShortenerService
{
    private readonly IUrlRepository _repo;
    private readonly ICacheService _cache;

    public UrlShortenerService(IUrlRepository repo, ICacheService cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<string> ShortenAsync(string originalUrl)
    {
        // Generate a 7-char Base62 code from a GUID
        var code = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                          .Replace("+", "").Replace("/", "").Replace("=", "")
                          .Substring(0, 7);

        var entity = new ShortUrl { OriginalUrl = originalUrl, ShortCode = code };
        await _repo.SaveAsync(entity);
        await _cache.SetAsync(code, originalUrl, TimeSpan.FromMinutes(5));
        return code;
    }

    public async Task<string?> ResolveAsync(string code)
    {
        // Cache-aside: check Redis first, then PostgreSQL
        var cached = await _cache.GetAsync(code);
        if (cached != null) return cached;

        var entity = await _repo.GetByCodeAsync(code);
        if (entity == null) return null;

        await _cache.SetAsync(code, entity.OriginalUrl, TimeSpan.FromMinutes(5));
        return entity.OriginalUrl;
    }
}