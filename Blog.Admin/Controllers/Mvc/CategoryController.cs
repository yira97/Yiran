using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Admin.Services;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

public class CategoryController : Controller
{
    private readonly BlogService _blogService;
    private readonly CommonLocalizationService _commonLocalizationService;

    public CategoryController(BlogService blogService, CommonLocalizationService commonLocalizationService)
    {
        _blogService = blogService;
        _commonLocalizationService = commonLocalizationService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");
        var domain = await _blogService.GetDomainAsync(domainId);

        var vm = new CategoryViewModel();
        vm.Categories = domain.Categories.ToList();
        ViewData[ViewHelper.ViewData.ActiveNav] = "类别";
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
            new NavigationDto(_commonLocalizationService.Get("类别"), Url.Action("Index", "Category", new { domainId })!),
            new NavigationDto(_commonLocalizationService.Get("新增类别"),
                Url.Action("AddCategory", "Category", new { domainId })!)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(AddCategoryViewModel vm)
    {

        var res = await _blogService.AddCategory(vm.DomainId, new DomainCategoryUpdateDto(Name: vm.Name));

        return RedirectToAction("Index", new { domainId = vm.DomainId });
    }

    public async Task<IActionResult> EditCategory(string id)
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domainDto = await _blogService.GetDomainAsync(domainId);
        var category = domainDto.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return NotFound();

        var vm = new EditCategoryViewModel();
        vm.Name = category.Name;
        vm.CategoryId = category.Id;
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto(_commonLocalizationService.Get("类别"), Url.Action("Index", "Category", new { domainId })!),
            new NavigationDto($"{_commonLocalizationService.Get("编辑类别")} ({category.Name})",
                Url.Action("EditCategory", "Category", new { domainId })!)
        });
        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> EditCategory(EditCategoryViewModel vm)
    {

        var res = await _blogService.EditCategory(vm.DomainId, vm.CategoryId,
            new DomainCategoryUpdateDto(Name: vm.Name));

        return RedirectToAction("Index", new { domainId = vm.DomainId });
    }

    /// <summary>
    /// 删除一个 Category
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(DeleteCategoryDto deleteCategoryDto)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        await _blogService.DeleteCategory(deleteCategoryDto.DomainId, deleteCategoryDto.CategoryId);

        return RedirectToAction("Index", new { domainId = deleteCategoryDto.DomainId });
    }
}