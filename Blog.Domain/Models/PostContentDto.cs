namespace Blog.Domain.Models;

public record PostContentDto
(
    ImageWithCaptionDto? Cover,
    IEnumerable<PostContentBlockDto> Blocks
);