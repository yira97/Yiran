using Blog.Admin.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Areas.Account.Controllers;

[Area("Account")]
public class LogOutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        CookieHelper.ClearLoginInfo(HttpContext);

        return RedirectToAction("Index", "Home", new { Area = "" });
    }
}