namespace UrlShortener.Core;

public class ShortUrl
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public long ClickCount { get; set; }
}
