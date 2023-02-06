using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Blog.Admin.Models;

public class PostEditViewModel
{
    public class InputModel
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string SubTitle { get; set; } = string.Empty;
        [Required] public string Slug { get; set; } = string.Empty;
        [Required] public string Topic { get; set; } = string.Empty;
        [Required] public string Category { get; set; } = string.Empty;
        [Required] public string Language { get; set; } = "zh";
        [Required] public bool IsPublic { get; set; } = true;
        [Required] public string PostContentJson { get; set; } = string.Empty;
        [Required] public string DomainId { get; set; } = string.Empty;
    }

    public InputModel PostEditFormData { get; set; } = new InputModel();

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = "zh";
    public bool IsPublic { get; set; } = true;
    public string DomainId { get; set; } = string.Empty;

    public List<SelectListItem> DomainCategories { get; set; } = new();
    public List<SelectListItem> DomainTopics { get; set; } = new();
}