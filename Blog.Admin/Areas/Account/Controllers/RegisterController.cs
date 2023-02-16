using Blog.Admin.Areas.Account.VIewModels.Register;
using Blog.Admin.Helper;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Areas.Account.Controllers;

[Area("Account")]
public class RegisterController : Controller
{
    private readonly BlogService _blogService;

    public RegisterController(BlogService blogService)
    {
        _blogService = blogService;
    }

    public IActionResult Index()
    {
        var vm = new IndexViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Index(IndexViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var input = vm.FormInput;

        var result =
            await _blogService.EmailRegister(new EmailPasswordAuthDto(Email: input.Email, Password: input.Password));

        CookieHelper.WriteAccessTokenToCookie(HttpContext, result);

        var userProfile = await _blogService.GetUserProfile(result.AccessToken!);

        CookieHelper.WriteUserInfoToCookie(HttpContext, userProfile);

        return RedirectToAction("Index", "Home", new { Area = "" });
    }
}