using Blog.Domain.Models;

namespace Blog.Web.ViewModels.ViewComponents;

public class NavViewModel
{
    public DomainDto? DomainInfo { get; set; }
    public string? CurrentTopic { get; set; }
}