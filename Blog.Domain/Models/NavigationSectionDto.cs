namespace Blog.Domain.Models;

public class NavigationSectionDto
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<NavigationDto> Links { get; set; } = new List<NavigationDto>();
}