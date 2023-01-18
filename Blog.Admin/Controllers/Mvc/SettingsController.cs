using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

public class SettingsController : Controller
{
    private readonly ILogger<PostController> _logger;
    private readonly BlogService _blogService;

    public SettingsController(ILogger<PostController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }
}