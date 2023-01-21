using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Index()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");
        var domain = await _blogService.GetDomain(domainId);

        var vm = new CategoryViewModel();
        vm.Categories = domain.Categories.ToList();
        ViewData[ViewHelper.ViewData.ActiveNav] = RouteHelper.Controller.Category;
        return View(vm);
    }

    public async Task<IActionResult> AddCategory()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var vm = new AddCategoryViewModel();
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Category", Url.Action("Index", "Category")!, false),
            new NavigationDto($"Add Category", Url.Action("AddCategory", "Category")!, true)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(AddCategoryViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        if (string.IsNullOrEmpty(accessToken.AccessToken)) return RedirectToAction("Register", "Account");

        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var res = await _blogService.AddCategory(domainId, new DomainCategoryUpdateDto(Name: vm.Name),
            accessToken.AccessToken!);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditCategory(string categoryId)
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domainDto = await _blogService.GetDomain(domainId);
        var category = domainDto.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null) return NotFound();

        var vm = new EditCategoryViewModel();
        vm.Name = category.Name;
        vm.CategoryId = category.Id;
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Category", Url.Action("Index", "Category")!, false),
            new NavigationDto($"Edit Category ({category.Name})", Url.Action("EditCategory", "Category")!, true)
        });
        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> EditCategory(EditCategoryViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();

        var res = await _blogService.EditCategory(vm.DomainId, vm.CategoryId,
            new DomainCategoryUpdateDto(Name: vm.Name),
            accessToken.AccessToken!);

        return RedirectToAction("Index");
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