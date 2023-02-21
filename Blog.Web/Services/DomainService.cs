using Blog.Domain.Models;
using Blog.Domain.Services.Client;

namespace Blog.Web.Services;

public class DomainService : IDomainService
{
    private const string SiteMapInfoDefault = "_default";

    private readonly BlogService _blogService;
    private string DomainName { get; set; }
    private TimeSpan RefreshSpan { get; set; } = TimeSpan.FromSeconds(1);
    private DomainDto? DomainInfo { get; set; }
    private DateTime DomainInfoRefreshedAt { get; set; } = DateTime.UtcNow;

    private SocialLinksDto? SocialLinksInfo { get; set; }
    private DateTime SocialLinksInfoRefreshedAt { get; set; } = DateTime.UtcNow;
    private Dictionary<string, SiteMapDto?> SiteMapInfos { get; set; }
    private DateTime SiteMapInfosRefreshedAt { get; set; } = DateTime.UtcNow;

    public DomainService(BlogService blogService, IConfiguration configuration)
    {
        _blogService = blogService;
        DomainName = configuration.GetSection("Domain").Get<string>()!;
        SiteMapInfos = new Dictionary<string, SiteMapDto?>();
    }

    private async Task<DomainDto> UpdateDomainInfo()
    {
        var info = await _blogService.GetDomainByNameAsync(DomainName);
        DomainInfo = info;
        return DomainInfo;
    }

    public async Task<DomainDto> GetDomainInfo()
    {
        if (DomainInfo != null && DomainInfoRefreshedAt - DateTime.UtcNow < RefreshSpan) return DomainInfo;
        var domainInfo = await UpdateDomainInfo();
        return domainInfo;
    }

    private async Task<SocialLinksDto> UpdateSocialLinksInfo()
    {
        var domainInfo = await GetDomainInfo();
        var domainId = domainInfo.Id;
        var info = await _blogService.GetSocialLinks(domainId);
        SocialLinksInfo = info;
        return SocialLinksInfo;
    }

    public async Task<SocialLinksDto> GetSocialLinksInfo()
    {
        if (SocialLinksInfo != null && SocialLinksInfoRefreshedAt - DateTime.UtcNow < RefreshSpan)
            return SocialLinksInfo;
        var socialLinksInfo = await UpdateSocialLinksInfo();
        return socialLinksInfo;
    }

    private async Task UpdateSiteMapInfo(string language)
    {
        var domainInfo = await GetDomainInfo();
        var domainId = domainInfo.Id;
        var info = await _blogService.GetSiteMap(domainId, language);
        SiteMapInfos[language] = info;
        SiteMapInfos[language] = info;
    }

    private async Task UpdateDefaultSiteMapInfo()
    {
        var domainInfo = await GetDomainInfo();
        var domainId = domainInfo.Id;
        var info = await _blogService.GetSiteMap(domainId);
        SiteMapInfos[SiteMapInfoDefault] = info;
    }

    public async Task<SiteMapDto> GetSiteMapInfo(string language)
    {
        // 确保默认语言进行过查询
        if (!SiteMapInfos.ContainsKey(SiteMapInfoDefault) || SiteMapInfosRefreshedAt - DateTime.UtcNow > RefreshSpan)
        {
            await UpdateDefaultSiteMapInfo();
        }

        // 确保该语言进行过查询
        if (!SiteMapInfos.ContainsKey(language) || SiteMapInfosRefreshedAt - DateTime.UtcNow > RefreshSpan)
        {
            await UpdateSiteMapInfo(language);
        }

        return SiteMapInfos[language] ?? SiteMapInfos[SiteMapInfoDefault] ?? new SiteMapDto();
    }
}