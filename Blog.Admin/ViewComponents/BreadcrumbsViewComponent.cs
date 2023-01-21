using Blog.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.ViewComponents;

public class BreadcrumbsViewComponent : ViewComponent
{
    public BreadcrumbsViewComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync(
        BreadcrumbsDto breadcrumbs)
    {
        var vm = new BreadcrumbsViewModel(breadcrumbs);

        return View(vm);
    }
}