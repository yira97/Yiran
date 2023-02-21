using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IDomainRepository
{
    Task<IEnumerable<DomainDto>> ListAllDomains();
    DomainDto CreateDomain(DomainUpdateDto updateDto, string userId);
    Task<DomainDto?> GetDomainInfo(string domainId);

    Task<DomainDto?> GetDomainInfoByName(string domainName);

    Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto, string userId);
    Task<bool> DeleteDomain(string domainId, string userId);


    Task<DomainCategoryDto> AddDomainCategory(string domainId, DomainCategoryUpdateDto updateDto,
        string userId);

    Task<DomainCategoryDto> UpdateDomainCategory(string domainCategoryId,
        DomainCategoryUpdateDto updateDto, string userId);

    Task<bool> DeleteDomainCategoryImmediately(string domainCategoryId, string userId);

    Task<DomainTopicDto> AddDomainTopic(string domainId, DomainTopicUpdateDto updateDto, string userId);

    Task<DomainTopicDto> UpdateDomainTopic(string domainTopicId,
        DomainTopicUpdateDto updateDto, string userId);

    Task<bool> DeleteDomainTopicImmediately(string domainTopicId, string userId);

    Task<SiteMapDto?> GetSiteMap(string domainId);

    Task<SiteMapDto?> GetSiteMap(string domainId, string language);

    Task<SiteMapDto> UpdateSiteMap(string domainId, SiteMapDto siteMap, string userId);

    Task<SiteMapDto> UpdateSiteMapTranslation(string domainId, SiteMapDto siteMap, string userId);

    Task<SocialLinksDto> UpdateSocialLinks(string domainId, SocialLinksDto socialDto, string userId);

    Task<SocialLinksDto> GetSocialLinks(string domainId);
}