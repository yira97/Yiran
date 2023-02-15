using Blog.Web.Services;
using Blog.Web.ViewModels.ViewComponents;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ViewComponents;

public class SiteMapDisplayViewComponent : ViewComponent
{
    private readonly IDomainService _domainService;

    public SiteMapDisplayViewComponent(IDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var vm = new SiteMapDisplayViewModel();
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        if (cultureFeature != null)
        {
            vm.SiteMap = await _domainService.GetSiteMapInfo(cultureFeature.RequestCulture.UICulture.Name);
        }

        return View(vm);
    }
}