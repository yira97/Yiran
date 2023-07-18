using System.ComponentModel.DataAnnotations;

namespace Blog.Domain.Models;

public class PostUpdateRequestDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string SubTitle { get; set; }

    [Required]
    public string Slug { get; set; }

    [Required]
    public string Topic { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public string Language { get; set; }

    [Required]
    public bool IsPublic { get; set; }

    [Required]
    public PostContentDto Content { get; set; }

    public string DomainId { get; set; } = string.Empty;
}