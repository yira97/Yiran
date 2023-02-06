using Blog.Admin.Helper;
using Blog.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.ViewComponents;

public class UserAccountDropDownViewComponent : ViewComponent
{
    public UserAccountDropDownViewComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync(
        string type = "Default")
    {
        var userInfo = CookieHelper.GetUserInfoFromCookie(HttpContext);

        var vm = new UserAccountDropDownViewModel
        {
            DisplayName = userInfo.DisplayName ?? "NO_DISPLAY_NAME",
            Email = "NO_EMAIL",
            UserNavigation = new List<NavigationDto>
            {
                new NavigationDto(Name: "Home", Href: Url.Action("Index", "Home")!),
                new NavigationDto(Name: "Domain", Href: Url.Action("Index", "Domain")!),
                new NavigationDto(Name: "Logout", Href: Url.Action("Logout", "Account")!),
            }
        };

        return View(type, vm);
    }
}