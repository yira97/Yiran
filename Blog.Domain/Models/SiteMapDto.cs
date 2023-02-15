namespace Blog.Domain.Models;

public class SiteMapDto
{
    public string Language { get; set; } = Enums.Language.English;
    public List<NavigationSectionDto> Sections { get; set; } = new List<NavigationSectionDto>();
}