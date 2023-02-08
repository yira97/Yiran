using Blog.Web.Services;
using Blog.Web.ViewModels.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.ViewComponents;

public class NavViewComponent : ViewComponent
{
    private readonly IDomainService _domainService;

    public NavViewComponent(IDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string currentTopic)
    {
        var vm = new NavViewModel();
        vm.DomainInfo = await _domainService.GetInfo();
        vm.CurrentTopic = currentTopic;
        return View(vm);
    }
}