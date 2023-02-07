using System.Globalization;
using Blog.Admin.Areas.Account.VIewModels.Settings;
using Blog.Admin.Helper;
using Blog.Admin.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Blog.Admin.Areas.Account.Controllers;

[Area("Account")]
public class SettingsController : Controller
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public SettingsController(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    // GET
    public IActionResult Index()
    {
        var vm = new IndexViewModel();
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        if (cultureFeature != null)
        {
            vm.Culture = cultureFeature.RequestCulture.UICulture.DisplayName;
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult UpdateLanguage()
    {
        var vm = new UpdateLanguageViewModel();
        vm.Languages = _localizationOptions.Value.SupportedUICultures!.Select(culture => new SelectListItem(
            text: culture.DisplayName, value: culture.Name));

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Account Setting", Url.Action("Index", "Settings", new { Area = "Account" })!),
            new NavigationDto("Update Language", Url.Action("UpdateLanguage", "Settings", new { Area = "Account" })!)
        });

        return View(vm);
    }

    [HttpPost]
    public IActionResult UpdateLanguage(UpdateLanguageViewModel vm)
    {
        var input = vm.FormInput;
        CookieHelper.WriteCultureToCookie(HttpContext, new CultureInfo(input.Language));
        return RedirectToAction("Index", "Settings", new { Area = "Account" });
    }
}