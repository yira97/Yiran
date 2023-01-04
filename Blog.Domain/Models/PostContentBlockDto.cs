namespace Blog.Domain.Models;

public record PostContentBlockDto(
    string? Paragraph = null,
    ImageWithCaptionDto? Image = null,
    IEnumerable<ImageWithCaptionDto>? Images = null
);