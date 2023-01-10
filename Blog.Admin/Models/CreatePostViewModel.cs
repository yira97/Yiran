using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class CreatePostViewModel
{
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = "zh";
    public bool IsPublic { get; set; } = true;
    public ImageWithCaptionDto? Cover { get; set; }
    public List<PostContentBlockDto> Blocks = new List<PostContentBlockDto>();
    public string? DomainId { get; set; }
}