using Blog.Admin.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Admin.Controllers.Mvc;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BlogService _blogService;

    public HomeController(ILogger<HomeController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    public async Task<IActionResult> Index()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        return View();
    }
}