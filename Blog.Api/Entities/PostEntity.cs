using Blog.Api.Entities.Nested;

namespace Blog.Api.Entities;

public class PostEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;
    public string UpdatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Language { get; set; } = string.Empty;
    public bool isPublic { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public PostContent Content { get; set; } = new PostContent();

    // relationship
    public string DomainId { get; set; } = string.Empty;
    public DomainEntity? Domain { get; set; }
}