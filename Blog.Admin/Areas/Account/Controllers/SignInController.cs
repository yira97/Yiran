using Blog.Admin.Areas.Account.VIewModels.SignIn;
using Blog.Admin.Helper;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Areas.Account.Controllers;

[Area("Account")]
public class SignInController : Controller
{
    private readonly BlogService _blogService;

    public SignInController(BlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<IActionResult> Index()
    {
        var exist = await _blogService.ExistAdmin();
        var vm = new IndexViewModel();
        vm.AcceptRegister = !exist;
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
            await _blogService.EmailLogin(new EmailPasswordAuthDto(Email: input.Email, Password: input.Password));

        CookieHelper.WriteAccessTokenToCookie(HttpContext, result);

        var userProfile = await _blogService.GetUserProfile(result.AccessToken!);

        CookieHelper.WriteUserInfoToCookie(HttpContext, userProfile);

        return RedirectToAction("Index", "Home", new { Area = "" });
    }
}