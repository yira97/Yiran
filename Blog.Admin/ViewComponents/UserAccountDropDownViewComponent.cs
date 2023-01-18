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
        string currentDomainId)
    {
        var userInfo = CookieHelper.GetUserInfoFromCookie(HttpContext);

        var vm = new UserAccountDropDownViewModel
        {
            DisplayName = userInfo.DisplayName,
            Email = userInfo.Email,
        };

        return View(vm);
    }
}