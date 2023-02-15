using Blog.Domain.Models;

namespace Blog.Api.Entities;

public class SiteMapEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DomainEntity Domain { get; set; } = default!;

    public string? TextContentId { get; set; }
}