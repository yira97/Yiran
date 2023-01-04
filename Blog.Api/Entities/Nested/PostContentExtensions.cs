using Blog.Domain.Models;

namespace Blog.Api.Entities.Nested;

public static class PostContentExtensions
{
    public static PostContentDto PostContentDto(this PostContent src)
    {
        var dto = new PostContentDto(
            Cover: src.Cover?.ImageWithCaptionDto(),
            Blocks: src.Blocks.Select(b => b.PostContentBlockDto())
        );

        return dto;
    }

    public static PostContent GeneratePostContent(this PostContentDto dto)
    {
        var model = new PostContent();

        model.Cover = dto.Cover?.GenerateImageWithCaption();
        model.Blocks = dto.Blocks.Select(b => b.GeneratePostContentBlock()).ToList();

        return model;
    }
}