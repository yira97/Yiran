using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

public class ContactController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}