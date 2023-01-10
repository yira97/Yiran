namespace Blog.Domain.Models;

public record PostDto
(
    string Id,
    string Title,
    string SubTitle,
    string Slug,
    string Topic,
    string Category,
    string CreatedById,
    string UpdatedById,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Language,
    bool isPublic,
    PostContentDto Content
);