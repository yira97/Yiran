namespace Blog.Api.Models;

public record PostContentDto
(
    ImageWithCaptionDto? Cover,
    IEnumerable<PostContentBlockDto> Blocks
);