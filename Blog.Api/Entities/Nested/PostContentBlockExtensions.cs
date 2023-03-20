using Blog.Domain.Models;

namespace Blog.Api.Entities.Nested;

public static class PostContentBlockExtensions
{
    public static PostContentBlockDto PostContentBlockDto(this PostContentBlock src)
    {

        CodeSnippetDto? code = null;
        if (src.Code != null)
        {
            code = new CodeSnippetDto(
                FileName: src.Code.FileName,
                Language: src.Code.Language,
                Content: src.Code.Content
            );
        }
        
        var dto = new PostContentBlockDto(
            Paragraph: src.Paragraph,
            Images: src.Images?.Select(i => i.ImageWithCaptionDto()),
            Code: code
        );

        return dto;
    }

    public static PostContentBlock GeneratePostContentBlock(this PostContentBlockDto dto)
    {
        var model = new PostContentBlock();

        model.Images = dto.Images?.Select(i => i.GenerateImageWithCaption()).ToList();
        model.Paragraph = dto.Paragraph;

        if (dto.Code != null)
        {
            model.Code = new CodeSnippet
            {
                FileName = dto.Code.FileName,
                Language = dto.Code.Language,
                Content = dto.Code.Content
            };
        }
        return model;
    }
}