using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Models;

public class EditDomainViewModel
{
    public string Id = string.Empty;
    [Required] [MinLength(1)] public string Name { get; set; } = string.Empty;
}