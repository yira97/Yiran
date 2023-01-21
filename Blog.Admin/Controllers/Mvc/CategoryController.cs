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
            new NavigationDto("Category", Url.Action("Index", "Category", new { domainId })!, false),
            new NavigationDto($"Add Category", Url.Action("AddCategory", "Category", new { domainId })!, true)
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

        return RedirectToAction("Index", new { domainId });
    }

    public async Task<IActionResult> EditCategory(string id)
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domainDto = await _blogService.GetDomain(domainId);
        var category = domainDto.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return NotFound();

        var vm = new EditCategoryViewModel();
        vm.Name = category.Name;
        vm.CategoryId = category.Id;
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Category", Url.Action("Index", "Category", new { domainId })!, false),
            new NavigationDto($"Edit Category ({category.Name})",
                Url.Action("EditCategory", "Category", new { domainId })!, true)
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

        return RedirectToAction("Index", new { domainId = vm.DomainId });
    }

    /// <summary>
    /// 删除一个 Category
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(DeleteCategoryDto deleteCategoryDto)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        if (string.IsNullOrEmpty(accessToken.RefreshToken)) return RedirectToAction("Register", "Account");

        await _blogService.DeleteCategory(deleteCategoryDto.DomainId, deleteCategoryDto.CategoryId,
            accessToken.AccessToken!);

        return RedirectToAction("Index", new { domainId = deleteCategoryDto.DomainId });
    }
}