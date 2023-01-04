namespace Blog.Domain.Models;

public record PostUpdateDto
(
    string Title,
    string SubTitle,
    string Slug,
    int Topic,
    int Category,
    string Language,
    bool IsPublic,
    PostContentDto Content,
    string? DomainId = null
);