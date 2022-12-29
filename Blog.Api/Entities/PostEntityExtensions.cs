using Blog.Api.Entities.Nested;
using Blog.Api.Models;

namespace Blog.Api.Entities;

public static class PostEntityExtensions
{

    public static PostDto PostDto(this PostEntity src)
    {
        var dto = new PostDto(
            Id: src.Id,
            Title: src.Title,
            SubTitle: src.SubTitle,
            Slug: src.Slug,
            Topic: src.Topic,
            Category: src.Category,
            CreatedById: src.CreatedById,
            UpdatedById: src.UpdatedById,
            CreatedAt: src.CreatedAt,
            UpdatedAt: src.UpdatedAt,
            Language: src.Language,
            isPublic: src.isPublic,
            Content: src.Content.PostContentDto()
        );

        return dto;
    }
}