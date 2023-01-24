using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Blog.Admin.Models;

public class EditPostViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string SubTitle { get; set; } = string.Empty;
    [Required] public string Slug { get; set; } = string.Empty;

    public string Topic { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [Required] public string Language { get; set; } = "zh";

    public bool IsPublic { get; set; } = true;

    public ImageWithCaptionDto? Cover { get; set; }

    public List<PostContentBlockDto> Blocks = new List<PostContentBlockDto>();

    public string? DomainId { get; set; }

    public List<SelectListItem> DomainCategories { get; set; } = new();
    public List<SelectListItem> DomainTopics { get; set; } = new();
}