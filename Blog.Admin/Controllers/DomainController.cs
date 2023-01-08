using Blog.Admin.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers;

public class DomainController : Controller
{
    private readonly ILogger<DomainController> _logger;
    private readonly BlogService _blogService;

    public DomainController(ILogger<DomainController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    public IActionResult Create()
    {
        var vm = new CreateDomainViewModel();

        ViewData["Levels"] = new List<string> { "Domains", "Create Domain" };
        ViewData["LevelLinks"] = new List<string> { Url.Action("Domain", "Home")!, Url.Action("Create")! };
        ViewData["ActiveLevel"] = 1;
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(CreateDomainViewModel vm)
    {
        return View(vm);
    }
}