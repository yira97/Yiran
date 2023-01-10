using Blog.Admin.Helper;
using Blog.Admin.Models;
using Blog.Domain.Extensions;
using Blog.Domain.Models;
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
        return View(vm);
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

    public async Task<IActionResult> AddTopic(string id)
    {
        var domainDto = await _blogService.GetDomain(id);

        var vm = new AddTopicViewModel();

        ViewData["Levels"] = new List<string>
        {
            "Domains",
            $"Domains({domainDto.Name})",
            "Add Topic",
        };
        ViewData["LevelLinks"] = new List<string>
        {
            Url.Action("Domain", "Home")!,
            Url.Action("Index", "Domain", new { id })!,
            Url.Action("AddTopic", "Domain", new { id })!,
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(string id, AddCategoryViewModel vm)
    {
    }

    [HttpPost]
    public async Task<IActionResult> AddTopic(string id, AddTopicViewModel vm)
    {
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateDomainViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var shape = new DomainUpdateDto(Name: vm.Name);

        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var domain = await _blogService.CreateDomain(shape, accessTokenDto.AccessToken);
        return RedirectToAction("Index", new { id = domain.Id });
    }

    public async Task<IActionResult> Index(string id)
    {
        var domainDto = await _blogService.GetDomain(id);

        var vm = new DomainDetailViewModel();
        vm.Domain = domainDto;

        ViewData["Levels"] = new List<string> { "Domains", $"Domain({domainDto.Name})" };
        ViewData["LevelLinks"] = new List<string>
            { Url.Action("Domain", "Home")!, Url.Action("Index", new { id = domainDto.Id })! };
        return View(vm);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var userId = HttpContext.User.GetUserId();
        var domain = await _blogService.GetDomain(id);
        var vm = new EditDomainViewModel();
        vm.Id = domain.Id;
        vm.Name = domain.Name;

        _logger.LogInformation($"user {userId} edited domain {domain.Id}");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, EditDomainViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        _ = await _blogService.UpdateDomain(id, new DomainUpdateDto(vm.Name), accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }
}