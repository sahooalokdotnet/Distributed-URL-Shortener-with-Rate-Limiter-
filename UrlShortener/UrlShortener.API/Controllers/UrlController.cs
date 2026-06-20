using Microsoft.AspNetCore.Mvc;
using UrlShortener.Core.Services;

namespace UrlShortener.UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly UrlShortenerService _service;

    public UrlController(UrlShortenerService service) => _service = service;

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest req)
    {
        if (!Uri.TryCreate(req.Url, UriKind.Absolute, out _))
            return BadRequest("Invalid URL");

        var code = await _service.ShortenAsync(req.Url);
        return Ok(new { shortUrl = $"{Request.Scheme}://{Request.Host}/{code}" });
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Redirect(string code)
    {
        var url = await _service.ResolveAsync(code);
        if (url == null) return NotFound();
        return Redirect(url);          // 302 redirect
    }

    [HttpGet("{code}/stats")]
    public async Task<IActionResult> Stats(string code)
    {
        var entity = await _service.GetStatsAsync(code);
        if (entity == null) return NotFound();
        return Ok(new { entity.ShortCode, entity.ClickCount, entity.CreatedAt });
    }
}