namespace Blog.Api.Models;

public record PostDto
(
    string Id,
    string Title,
    string SubTitle,
    string Slug,
    int Topic,
    int Category,
    string CreatedById,
    string UpdatedById,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Language,
    bool isPublic,
    PostContentDto Content
    );