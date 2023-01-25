using Blog.Domain.Models;

namespace Blog.Api.Entities.Nested;

public static class PostContentBlockExtensions
{
    public static PostContentBlockDto PostContentBlockDto(this PostContentBlock src)
    {
        var dto = new PostContentBlockDto(
            Paragraph: src.Paragraph,
            Images: src.Images?.Select(i => i.ImageWithCaptionDto())
        );

        return dto;
    }

    public static PostContentBlock GeneratePostContentBlock(this PostContentBlockDto dto)
    {
        var model = new PostContentBlock();

        model.Images = dto.Images?.Select(i => i.GenerateImageWithCaption()).ToList();
        model.Paragraph = dto.Paragraph;

        return model;
    }
}