using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class DomainController : ControllerBase
{
    private readonly BlogService _blogService;

    public DomainController(BlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet("{id}/site-map-content", Name = "GetSiteMapContent")]
    public async Task<ActionResult<SiteMapDto>> GetPostContent(string id)
    {
        var siteMap = await _blogService.GetSiteMap(id);
        if (siteMap == null) return NotFound();
        return siteMap;
    }

    [HttpGet("{id}/site-map-translation-content/{language}", Name = "GetSiteMapTranslationContent")]
    public async Task<ActionResult<SiteMapDto>> GetPostContent(string id, string language)
    {
        var siteMap = await _blogService.GetSiteMap(id, language);
        if (siteMap == null) return NotFound();
        return siteMap;
    }
}