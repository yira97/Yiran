using Blog.Web.Services;
using Blog.Web.ViewModels.ViewComponents;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Blog.Web.ViewComponents;

public class CultureDisplayViewComponent : ViewComponent
{
    public CultureDisplayViewComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var vm = new CultureDisplayViewModel();
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        if (cultureFeature != null)
        {
            vm.Culture = cultureFeature.RequestCulture.UICulture;
        }

        await Task.CompletedTask;
        
        return View(vm);
    }
}