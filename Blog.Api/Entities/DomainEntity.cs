using Blog.Domain.Models;

namespace Blog.Api.Entities;

public class DomainEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    public List<PostEntity> Posts = default!;

    public List<DomainCategoryEntity> Categories = default!;

    public List<DomainTopicEntity> Topics = default!;

    public SocialLinksDto SocialLinks = new SocialLinksDto(null, null, null, null, null, null, null);

    public string? SiteMapId;

    public SiteMapEntity? SiteMap;

    public string CreatedById { get; set; } = string.Empty;
    public string UpdatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}