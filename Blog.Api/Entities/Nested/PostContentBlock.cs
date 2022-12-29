namespace Blog.Api.Entities.Nested;

public class PostContentBlock
{
    public ImageWithCaption? Image { get; set; }
    public List<ImageWithCaption>? Images { get; set; }
    public string? Paragraph { get; set; }
}