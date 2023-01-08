using System.ComponentModel.DataAnnotations;

namespace Blog.Admin.Models;

public class CreateDomainViewModel
{
    [Required] [MinLength(1)] public string Name { get; set; } = string.Empty;
}