using System.Text.Json;
using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Admin.ViewModels;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Admin.Controllers.Mvc;

public class SettingsController : Controller
{
    public class Config
    {
        public const string TabGeneral = "General";
        public const string TabSocialLinks = "SocialLinks";
        public const string TabSiteMap = "SiteMap";

        public static List<string> Tabs = new List<string>
        {
            TabGeneral,
            TabSocialLinks,
            TabSiteMap,
        };

        public static string TabDisplayName(string tab)
        {
            return tab switch
            {
                TabGeneral => "一般",
                TabSocialLinks => "社交账号链接",
                TabSiteMap => "网站地图",
                _ => throw new ArgumentOutOfRangeException(nameof(tab), tab, null)
            };
        }
    }

    private readonly ILogger<SettingsController> _logger;
    private readonly BlogService _blogService;

    public SettingsController(ILogger<SettingsController> logger, BlogService blogService)
    {
        _logger = logger;
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = Config.TabGeneral)
    {
        var vm = new SettingsIndexViewModel();

        var domainId = HttpContext.GetDomainIdFromHttpContextItems();
        if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

        vm.DomainId = domainId;
        ViewData[nameof(vm.DomainId)] = vm.DomainId;

        vm.ActiveTab = tab;
        ViewData[nameof(vm.ActiveTab)] = vm.ActiveTab;

        if (tab == Config.TabSocialLinks)
        {
            vm.SocialLinksForm.DomainId = domainId;
            vm.SocialLinks = await _blogService.GetSocialLinks(domainId);
            vm.SocialLinksForm.LinkedIn = vm.SocialLinks.LinkedIn ?? string.Empty;
            vm.SocialLinksForm.Facebook = vm.SocialLinks.Facebook ?? string.Empty;
            vm.SocialLinksForm.Twitter = vm.SocialLinks.Twitter ?? string.Empty;
            vm.SocialLinksForm.BiliBili = vm.SocialLinks.BiliBili ?? string.Empty;
            vm.SocialLinksForm.Github = vm.SocialLinks.Github ?? string.Empty;
            vm.SocialLinksForm.Instagram = vm.SocialLinks.Instagram ?? string.Empty;
            vm.SocialLinksForm.Youtube = vm.SocialLinks.Youtube ?? string.Empty;
        }

        if (tab == Config.TabSiteMap)
        {
            vm.SiteMapUpdateFormInput.DomainId = domainId;
            vm.SiteMapTranslationUpdateFormInput.DomainId = domainId;
            vm.SiteMap = await _blogService.GetSiteMap(domainId);
        }

        ViewResult? view = tab switch
        {
            Config.TabGeneral => View("General", vm),
            Config.TabSiteMap => View("SiteMap", vm),
            Config.TabSocialLinks => View("SocialLinks", vm),
            _ => View("General", vm)
        };

        ViewData[ViewHelper.ViewData.ActiveNav] = "设置";

        return view;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSocialLinks(SettingsIndexViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        if (string.IsNullOrEmpty(accessToken.AccessToken))
            return RedirectToAction("Index", "SignIn", new { Area = "Account" });

        var domainId = vm.SocialLinksForm.DomainId;

        var socialLinks = new SocialLinksDto(
            Facebook: string.IsNullOrEmpty(vm.SocialLinksForm.Facebook) ? null : vm.SocialLinksForm.Facebook,
            LinkedIn: string.IsNullOrEmpty(vm.SocialLinksForm.LinkedIn) ? null : vm.SocialLinksForm.LinkedIn,
            Instagram: string.IsNullOrEmpty(vm.SocialLinksForm.Instagram) ? null : vm.SocialLinksForm.Instagram,
            Twitter: string.IsNullOrEmpty(vm.SocialLinksForm.Twitter) ? null : vm.SocialLinksForm.Twitter,
            Github: string.IsNullOrEmpty(vm.SocialLinksForm.Github) ? null : vm.SocialLinksForm.Github,
            Youtube: string.IsNullOrEmpty(vm.SocialLinksForm.Youtube) ? null : vm.SocialLinksForm.Youtube,
            BiliBili: string.IsNullOrEmpty(vm.SocialLinksForm.BiliBili) ? null : vm.SocialLinksForm.BiliBili
        );
        await _blogService.UpdateSocialLinks(domainId, socialLinks, accessToken.AccessToken);

        return RedirectToAction("Index", "Settings", new { domainId = domainId, tab = Config.TabSocialLinks });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSiteMap(SettingsIndexViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        if (string.IsNullOrEmpty(accessToken.AccessToken))
            return RedirectToAction("Index", "SignIn", new { Area = "Account" });

        var domainId = vm.SiteMapUpdateFormInput.DomainId;
        var siteMapJson = vm.SiteMapUpdateFormInput.SiteMapDataJson;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapJson, options);

        await _blogService.UpdateSiteMap(domainId, siteMap!, accessToken.AccessToken);

        return RedirectToAction("Index", "Settings", new { domainId = domainId, tab = Config.TabSiteMap });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSiteMapTranslation(SettingsIndexViewModel vm)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        if (string.IsNullOrEmpty(accessToken.AccessToken))
            return RedirectToAction("Index", "SignIn", new { Area = "Account" });

        var domainId = vm.SiteMapTranslationUpdateFormInput.DomainId;
        var siteMapJson = vm.SiteMapTranslationUpdateFormInput.SiteMapDataJson;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapJson, options);

        await _blogService.UpdateSiteMapTranslation(domainId, siteMap!.Language, siteMap!, accessToken.AccessToken);

        return RedirectToAction("Index", "Settings", new { domainId = domainId, tab = Config.TabSiteMap });
    }
}