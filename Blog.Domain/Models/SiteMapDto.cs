namespace Blog.Domain.Models;

public class SiteMapDto
{
    public string Language { get; set; } = Enums.Language.Chinese;
    public List<NavigationSectionDto> Sections { get; set; } = new List<NavigationSectionDto>();
}