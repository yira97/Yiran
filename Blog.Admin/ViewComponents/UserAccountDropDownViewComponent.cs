using Blog.Admin.Models;
using Blog.Admin.Services;
using Blog.Domain.Extensions;
using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.ViewComponents;

public class UserAccountDropDownViewComponent : ViewComponent
{
    private readonly CommonLocalizationService _commonLocalizationService;

    public UserAccountDropDownViewComponent(CommonLocalizationService commonLocalizationService)
    {
        _commonLocalizationService = commonLocalizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string type = "Default")
    {
        var vm = new UserAccountDropDownViewModel
        {
            DisplayName = string.IsNullOrEmpty(HttpContext.User.GetName())
                ? _commonLocalizationService.Get("未设置昵称")
                : HttpContext.User.GetName(),
            Email = string.IsNullOrEmpty(HttpContext.User.GetEmail()) ? _commonLocalizationService.Get("未设置邮箱") : HttpContext.User.GetEmail(),
            UserNavigation = new List<NavigationDto>
            {
                new NavigationDto(Name: _commonLocalizationService.Get("内容管理"),
                    Href: Url.Action("Index", "Home", new { Area = "" })!),
                new NavigationDto(Name: _commonLocalizationService.Get("域管理"),
                    Href: Url.Action("Index", "Domain", new { Area = "" })!),
                new NavigationDto(Name: _commonLocalizationService.Get("用户设置"),
                    Href: Url.Action("Index", "Settings", new { Area = "Account" })!),
                new NavigationDto(Name: _commonLocalizationService.Get("注销"),
                    Href: Url.Action("Index", "LogOut", new { Area = "Account" })!),
            }
        };

        return View(type, vm);
    }
}