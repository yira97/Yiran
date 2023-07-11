using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Areas.Account.Controllers;

[Area("Account")]
public class AccessDeniedController : Controller
{
    [HttpGet]
    public async Task Index()
    {
        
    }
}