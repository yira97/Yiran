namespace Blog.Domain.Models;

// TODO: 把 domainID 从 PostUpdateDto中拿出
public record PostUpdateDto
(
    string Title,
    string SubTitle,
    string Slug,
    string Topic,
    string Category,
    string Language,
    bool IsPublic,
    PostContentDto Content,
    string? DomainId = null
);