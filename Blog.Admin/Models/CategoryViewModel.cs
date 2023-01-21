using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class CategoryViewModel
{
    public List<DomainCategoryDto> Categories { get; set; } = new();
}