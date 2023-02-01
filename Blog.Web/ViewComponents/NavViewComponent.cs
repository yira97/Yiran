using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ViewComponents;

public class NavViewComponent : ViewComponent
{
    private readonly IDomainService _domainService;

    public NavViewComponent(IDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var domainInfo = await _domainService.GetInfo();
        return View(domainInfo);
    }
}