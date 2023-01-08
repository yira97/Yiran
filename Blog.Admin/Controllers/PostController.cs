using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}