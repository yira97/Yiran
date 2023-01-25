using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public string PostContentJson { get; set; } = string.Empty;
    public string DomainId { get; set; } = string.Empty;

    public List<SelectListItem> DomainCategories = new();
    public List<SelectListItem> DomainTopics = new();
}