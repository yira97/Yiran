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

        var (tokenUpdated, accessTokenDto) =
            await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
        if (tokenUpdated)
        {
            CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
        }

        var domain = await _blogService.CreateDomain(shape, accessTokenDto.AccessToken);
        return RedirectToAction("Index", new { id = domain.Id });
    }

    /// <summary>
    /// 返回 Domain 详情页面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 返回更新 Domain 页面
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
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