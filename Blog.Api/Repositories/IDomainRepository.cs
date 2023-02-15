using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IDomainRepository
{
    Task<SiteMapDto?> GetSiteMap(string domainId);

    Task<SiteMapDto?> GetSiteMap(string domainId, string language);

    Task<SiteMapDto> UpdateSiteMap(string domainId, SiteMapDto siteMap, string userId);

    Task<SiteMapDto> UpdateSiteMapTranslation(string domainId, SiteMapDto siteMap, string userId);

    Task<SocialLinksDto> UpdateSocialLinks(string domainId, SocialLinksDto socialDto, string userId);

    Task<SocialLinksDto> GetSocialLinks(string domainId);
}