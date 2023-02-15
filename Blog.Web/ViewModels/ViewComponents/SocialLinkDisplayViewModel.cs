using Blog.Domain.Models;

namespace Blog.Web.ViewModels.ViewComponents;

public class SocialLinkDisplayViewModel
{
    public SocialLinksDto SocialLinks { get; set; } = new SocialLinksDto(null, null, null, null, null, null, null);
}