using Blog.Domain.Models;

namespace Blog.Api.Entities.Nested;

public static class ImageWithCaptionExtensions
{
    public static ImageWithCaptionDto ImageWithCaptionDto(this ImageWithCaption src)
    {
        var dto = new ImageWithCaptionDto(
            ResourceId: src.ResourceId,
            Caption: src.Caption
        );
        return dto;
    }

    public static ImageWithCaption GenerateImageWithCaption(this ImageWithCaptionDto dto)
    {
        var model = new ImageWithCaption();

        model.ResourceId = dto.ResourceId;
        model.Caption = dto.Caption;

        return model;
    }
}