using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace Blog.Admin.Models;

public class PostCreateViewModel
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

    public InputModel PostCreateFormData { get; set; } = new InputModel();
    public string DomainId { get; set; } = string.Empty;

    public List<SelectListItem> DomainCategories = new();
    public List<SelectListItem> DomainTopics = new();
}