using Blog.Admin.Helper;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly BlogService _blogService;

    public AccountController(ILogger<AccountController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }


    public IActionResult Register()
    {
        var vm = new RegisterViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("Register", vm);
        }

        var result = await _blogService.EmailRegister(new EmailPasswordAuthDto(Email: vm.Email, Password: vm.Password));

        CookieHelper.WriteAccessTokenToCookie(HttpContext, result);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    public IActionResult Login()
    {
        var vm = new LoginViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", vm);
        }

        var result = await _blogService.EmailLogin(new EmailPasswordAuthDto(Email: vm.Email, Password: vm.Password));

        Response.Cookies.Append("X-Access-Token", result.AccessToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        Response.Cookies.Append("X-Refresh-Token", result.RefreshToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        Response.Cookies.Delete("X-Refresh-Token");

        return RedirectToAction("Index", "Home");
    }
}