namespace Blog.Domain.Models;

public record PostContentBlockDto(
    string? Paragraph = null,
    IEnumerable<ImageWithCaptionDto>? Images = null
);