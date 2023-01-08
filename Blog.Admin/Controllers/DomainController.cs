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
        ViewData["ActiveLevel"] = 1;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDomainViewModel vm)
    {
        var shape = new DomainUpdateDto(Name: vm.Name);
        var domain = await _blogService.CreateDomain(shape);
        return RedirectToAction("Index", new { id = domain.Id });
    }

    public async Task<IActionResult> Index(string id)
    {
        var domainDto = await _blogService.GetDomain(id);

        var vm = new DomainDetailViewModel();
        vm.Domain = domainDto;

        ViewData["Levels"] = new List<string> { "Domains", domainDto.Name };
        ViewData["LevelLinks"] = new List<string>
            { Url.Action("Domain", "Home")!, Url.Action("Index", new { id = domainDto.Id })! };
        ViewData["ActiveLevel"] = 1;
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
    public async Task<IActionResult> Edit(EditDomainViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        _ = await _blogService.UpdateDomain(vm.Id, new DomainUpdateDto(vm.Name));

        return RedirectToAction("Index", new { id = vm.Id });
    }
}