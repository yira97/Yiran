using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blog.Admin.Models;
using Blog.Domain.Extensions;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Admin.Controllers;

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

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Domain()
    {
        var domains = await _blogService.ListDomains();
        var vm = new DomainViewModel
        {
            Domains = domains
        };
        return View(vm);
    }

    public IActionResult Post()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}