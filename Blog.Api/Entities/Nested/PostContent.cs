namespace Blog.Api.Entities.Nested;

public class PostContent
{
    public ImageWithCaption? Cover { get; set; }
    public List<PostContentBlock> Blocks { get; set; } = new();
}