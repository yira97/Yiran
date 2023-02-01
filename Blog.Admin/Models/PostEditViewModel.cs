using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Blog.Admin.Models;

public class PostEditViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string SubTitle { get; set; } = string.Empty;
    [Required] public string Slug { get; set; } = string.Empty;
    [Required] public string Topic { get; set; } = string.Empty;
    [Required] public string Category { get; set; } = string.Empty;
    [Required] public string Language { get; set; } = "zh";
    [Required] public bool IsPublic { get; set; } = true;
    [Required] public string PostContentJson { get; set; } = string.Empty;
    public PostContentDto? PostContent { get; set; }

    public List<SelectListItem> DomainCategories { get; set; } = new();
    public List<SelectListItem> DomainTopics { get; set; } = new();
}