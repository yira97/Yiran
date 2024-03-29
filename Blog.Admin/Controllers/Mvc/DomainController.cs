using Blog.Admin.Helper;
using Blog.Admin.Models;
using Blog.Domain.Enums;
using Blog.Domain.Extensions;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

[Authorize(Policy = Policies.RequireAdminRole)]
public class DomainController : Controller
{
    private readonly ILogger<DomainController> _logger;
    private readonly BlogService _blogService;

    public DomainController(ILogger<DomainController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    public async Task<IActionResult> Index()
    {
        var domains = await _blogService.ListDomains();

        var defaultDomain = CookieHelper.GetDefaultDomainFromCookie(HttpContext);

        var vm = new DomainViewModel();
        vm.Domains = domains;
        vm.DefaultDomain = defaultDomain;

        return View(vm);
    }

    public IActionResult Create()
    {
        var vm = new CreateDomainViewModel();
        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Domain", Url.Action("Index", "Domain")!),
            new NavigationDto("Create Domain", Url.Action("Create", "Domain")!)
        });
        return View(vm);
    }

    /// <summary>
    /// 提交创建 Domain 表单
    /// </summary>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateDomainViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var shape = new DomainUpdateDto(Name: vm.Name);

        var domain = await _blogService.CreateDomain(shape);
        return RedirectToAction("Index", "Domain");
    }

    /// <summary>
    /// 返回更新 Domain 页面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var userId = HttpContext.User.GetUserId();
        var domain = await _blogService.GetDomainAsync(id);
        var vm = new EditDomainViewModel();
        vm.Id = domain.Id;
        vm.Name = domain.Name;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Domain", Url.Action("Index", "Domain")!),
            new NavigationDto($"Edit Domain ({domain.Name})", Url.Action("Edit", "Domain")!)
        });
        _logger.LogInformation($"user {userId} edited domain {domain.Id}");
        return View(vm);
    }

    /// <summary>
    /// 提交 Domain 更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Edit(string id, EditDomainViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        _ = await _blogService.UpdateDomain(id, new DomainUpdateDto(vm.Name));

        return RedirectToAction("Index", "Domain");
    }

    [HttpPost]
    public IActionResult SetDefaultDomain(string id)
    {
        CookieHelper.WriteDefaultDomainToCookie(HttpContext, id);
        return RedirectToAction("Index", "Domain");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDomain(string id)
    {

        await _blogService.DeleteDomain(id);

        return RedirectToAction("Index", "Domain");
    }
}