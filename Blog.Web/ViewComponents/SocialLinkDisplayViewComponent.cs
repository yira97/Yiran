using Blog.Web.Services;
using Blog.Web.ViewModels.ViewComponents;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ViewComponents;

public class SocialLinkDisplayViewComponent : ViewComponent
{
    private readonly IDomainService _domainService;

    public SocialLinkDisplayViewComponent(IDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var vm = new SocialLinkDisplayViewModel();
        vm.SocialLinks = await _domainService.GetSocialLinksInfo();

        return View(vm);
    }
}