using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Admin.Models;

public class PostCreateViewModel
{
    public class InputModel
    {
        [Display(Name = "标题"), Required] public string Title { get; set; } = string.Empty;
        [Display(Name = "副标题"), Required] public string SubTitle { get; set; } = string.Empty;
        [Display(Name = "识别链接"), Required] public string Slug { get; set; } = string.Empty;
        [Display(Name = "话题"), Required] public string Topic { get; set; } = string.Empty;
        [Display(Name = "类别"), Required] public string Category { get; set; } = string.Empty;
        [Display(Name = "语言"), Required] public string Language { get; set; } = Domain.Enums.Language.Chinese;
        [Display(Name = "公开"), Required] public bool IsPublic { get; set; } = false;
        public string PostContentJson { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
    }

    public InputModel PostCreateFormData { get; set; } = new InputModel();
    public string DomainId { get; set; } = string.Empty;

    public List<SelectListItem> DomainCategories = new();
    public List<SelectListItem> DomainTopics = new();

    public IEnumerable<SelectListItem> SupportLanguages { get; set; } = new List<SelectListItem>();
}