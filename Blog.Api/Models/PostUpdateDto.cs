namespace Blog.Api.Models;

public record PostUpdateDto
(
    string Title,
    string SubTitle,
    string Slug,
    int Topic,
    int Category,
    string Language,
    bool isPublic,
    PostContentDto Content,
    string? DomainId = null
);