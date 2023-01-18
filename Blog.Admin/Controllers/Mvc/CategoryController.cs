using Blog.Admin.Helper;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

public class CategoryController : Controller
{
    private readonly BlogService _blogService;

    public CategoryController(BlogService blogService)
    {
        _blogService = blogService;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> AddCategory(string id)
    {
        var domainDto = await _blogService.GetDomain(id);

        var vm = new AddCategoryViewModel();

        ViewData["Levels"] = new List<string>
        {
            "Domains",
            $"Domains({domainDto.Name})",
            "Add Category",
        };
        ViewData["LevelLinks"] = new List<string>
        {
            Url.Action("Domain", "Home")!,
            Url.Action("Index", "Domain", new { id })!,
            Url.Action("AddCategory", "Domain", new { id })!,
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(string id, AddCategoryViewModel vm)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var res = await _blogService.AddCategory(id, new DomainCategoryUpdateDto(Name: vm.Name),
            accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }

    public async Task<IActionResult> EditCategory(string id, string categoryId)
    {
        var domainDto = await _blogService.GetDomain(id);
        var category = domainDto.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null) return NotFound();

        var vm = new EditCategoryViewModel();
        vm.Name = category.Name;
        vm.CategoryId = category.Id;
        vm.DomainId = domainDto.Name;

        ViewData["Levels"] = new List<string>
        {
            "Domains",
            $"Domains({domainDto.Name})",
            "Edit Category",
        };
        ViewData["LevelLinks"] = new List<string>
        {
            Url.Action("Domain", "Home")!,
            Url.Action("Index", "Domain", new { id })!,
            Url.Action("EditCategory", "Domain", new { id })!,
        };
        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> EditCategory(string id, EditCategoryViewModel vm)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var res = await _blogService.EditCategory(id, vm.CategoryId, new DomainCategoryUpdateDto(Name: vm.Name),
            accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }

    /// <summary>
    /// 删除一个 Category
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(string id, string categoryId)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        await _blogService.DeleteCategory(id, categoryId, accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }
}