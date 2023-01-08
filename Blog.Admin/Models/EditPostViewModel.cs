using Blog.Domain.Models;
using Microsoft.Build.Framework;

namespace Blog.Admin.Models;

public class EditPostViewModel
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string SubTitle { get; set; } = string.Empty;
    [Required] public string Slug { get; set; } = string.Empty;

    public int Topic { get; set; }

    public int Category { get; set; }
    [Required] public string Language { get; set; } = "zh";

    public bool IsPublic { get; set; } = true;

    public ImageWithCaptionDto? Cover { get; set; }

    public List<PostContentBlockDto> Blocks = new List<PostContentBlockDto>();

    [Required] public string? DomainId { get; set; }
}