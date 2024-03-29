using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Admin.Services;
using Blog.Domain.Enums;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

[Authorize(Policy = Policies.RequireAdminRole)]
public class TopicController : Controller
{
    private readonly BlogService _blogService;
    private readonly CommonLocalizationService _commonLocalizationService;

    public TopicController(BlogService blogService, CommonLocalizationService commonLocalizationService)
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

        var vm = new TopicViewModel();
        vm.Topics = domain.Topics.ToList();
        ViewData[ViewHelper.ViewData.ActiveNav] = "话题";
        return View(vm);
    }

    public async Task<IActionResult> AddTopic()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var vm = new AddTopicViewModel();
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto(_commonLocalizationService.Get("话题"), Url.Action("Index", "Topic", new { domainId })!),
            new NavigationDto(_commonLocalizationService.Get("新增话题"),
                Url.Action("AddTopic", "Topic", new { domainId })!)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddTopic(AddTopicViewModel vm)
    {
        var res = await _blogService.AddTopic(vm.DomainId, new DomainTopicUpdateDto(Name: vm.Name));

        return RedirectToAction("Index", new { domainId = vm.DomainId });
    }

    public async Task<IActionResult> EditTopic(string id)
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domainDto = await _blogService.GetDomainAsync(domainId);
        var topic = domainDto.Topics.FirstOrDefault(c => c.Id == id);
        if (topic == null) return NotFound();

        var vm = new EditTopicViewModel();
        vm.Name = topic.Name;
        vm.TopicId = topic.Id;
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto(_commonLocalizationService.Get("话题"), Url.Action("Index", "Topic", new { domainId })!),
            new NavigationDto($"{_commonLocalizationService.Get("编辑话题")} ({topic.Name})",
                Url.Action("EditTopic", "Topic", new { domainId })!)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditTopic(EditTopicViewModel vm)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        var res = await _blogService.EditTopic(vm.DomainId, vm.TopicId, new DomainTopicUpdateDto(Name: vm.Name));

        return RedirectToAction("Index", new { domainId = vm.DomainId });
    }

    /// <summary>
    /// 删除一个 Topic
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteTopic(DeleteTopicDto deleteTopicDto)
    {


        await _blogService.DeleteTopic(deleteTopicDto.DomainId, deleteTopicDto.TopicId);

        return RedirectToAction("Index", new { domainId = deleteTopicDto.DomainId });
    }
}