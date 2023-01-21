using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Mvc;

public class TopicController : Controller
{
    private readonly BlogService _blogService;

    public TopicController(BlogService blogService)
    {
        _blogService = blogService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domain = await _blogService.GetDomain(domainId);

        var vm = new TopicViewModel();
        vm.Topics = domain.Topics.ToList();
        ViewData[ViewHelper.ViewData.ActiveNav] = RouteHelper.Controller.Topic;
        return View();
    }

    public async Task<IActionResult> AddTopic()
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");
        var domain = await _blogService.GetDomain(domainId);

        var vm = new AddTopicViewModel();

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Topic", Url.Action("Index", "Topic")!, false),
            new NavigationDto($"Add Topic", Url.Action("AddTopic", "Topic")!, true)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddTopic(AddTopicViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();

        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var res = await _blogService.AddTopic(domainId, new DomainTopicUpdateDto(Name: vm.Name),
            accessToken.AccessToken);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditTopic(string topicId)
    {
        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        var domainDto = await _blogService.GetDomain(domainId);
        var topic = domainDto.Topics.FirstOrDefault(c => c.Id == topicId);
        if (topic == null) return NotFound();

        var vm = new EditTopicViewModel();
        vm.Name = topic.Name;
        vm.TopicId = topic.Id;
        vm.DomainId = domainId;

        ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
        {
            new NavigationDto("Topic", Url.Action("Index", "Topic")!, false),
            new NavigationDto($"Edit Topic ({topic.Name})", Url.Action("EditTopic", "Topic")!, true)
        });
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditTopic(EditTopicViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();

        var res = await _blogService.EditTopic(vm.DomainId, vm.TopicId, new DomainTopicUpdateDto(Name: vm.Name),
            accessToken.AccessToken!);

        return RedirectToAction("Index");
    }

    /// <summary>
    /// 删除一个 Topic
    /// </summary>
    /// <param name="id"></param>
    /// <param name="topicId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> DeleteTopic(string id, string topicId)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        await _blogService.DeleteTopic(id, topicId,
            accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }
}