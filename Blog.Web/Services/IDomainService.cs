using Blog.Domain.Models;

namespace Blog.Web.Services;

public interface IDomainService
{
    public Task<DomainDto> GetDomainInfo();

    public Task<SocialLinksDto> GetSocialLinksInfo();

    public Task<SiteMapDto> GetSiteMapInfo(string language);
}