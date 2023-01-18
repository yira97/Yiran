using Blog.Admin.Helper;
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
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> AddTopic(string id)
    {
        var domainDto = await _blogService.GetDomain(id);

        var vm = new AddTopicViewModel();

        ViewData["Levels"] = new List<string>
        {
            "Topic",
            "Add Topic",
        };
        ViewData["LevelLinks"] = new List<string>
        {
            Url.Action("Index", "Topic")!,
            Url.Action("AddTopic", "Topic", new { id })!,
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddTopic(string id, AddTopicViewModel vm)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var res = await _blogService.AddTopic(id, new DomainTopicUpdateDto(Name: vm.Name),
            accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
    }

    public async Task<IActionResult> EditTopic(string id, string topicId)
    {
        var domainDto = await _blogService.GetDomain(id);
        var topic = domainDto.Topics.FirstOrDefault(c => c.Id == topicId);
        if (topic == null) return NotFound();

        var vm = new EditTopicViewModel();
        vm.Name = topic.Name;

        ViewData["Levels"] = new List<string>
        {
            "Domains",
            $"Domains({domainDto.Name})",
            "Edit Topic",
        };
        ViewData["LevelLinks"] = new List<string>
        {
            Url.Action("Domain", "Home")!,
            Url.Action("Index", "Domain", new { id })!,
            Url.Action("EditTopic", "Domain", new { id })!,
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> EditTopic(string id, EditTopicViewModel vm)
    {
        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var res = await _blogService.EditTopic(id, vm.TopicId, new DomainTopicUpdateDto(Name: vm.Name),
            accessTokenDto.AccessToken);

        return RedirectToAction("Index", new { id });
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