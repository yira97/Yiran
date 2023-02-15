using System.ComponentModel.DataAnnotations;
using Blog.Admin.Controllers.Mvc;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Admin.ViewModels;

public class SettingsIndexViewModel
{
    public string DomainId { get; set; } = string.Empty;
    public string ActiveTab { get; set; } = string.Empty;
    public SocialLinksDto? SocialLinks { get; set; }
    public SiteMapDto? SiteMap { get; set; }

    public class SiteMapUpdateInput
    {
        public string DomainId { get; set; } = string.Empty;

        public string SiteMapDataJson { get; set; } = string.Empty;
    }

    public SiteMapUpdateInput SiteMapUpdateFormInput { get; set; } = new SiteMapUpdateInput();

    public class SiteMapTranslationUpdateInput
    {
        public string DomainId { get; set; } = string.Empty;

        public string SiteMapDataJson { get; set; } = string.Empty;
    }

    public SiteMapTranslationUpdateInput SiteMapTranslationUpdateFormInput { get; set; } =
        new SiteMapTranslationUpdateInput();

    public class SocialLinksInput
    {
        public string DomainId { get; set; } = string.Empty;

        [Url] public string Facebook = string.Empty;

        [Url] public string LinkedIn = string.Empty;

        [Url] public string Instagram = string.Empty;

        [Url] public string Twitter = string.Empty;

        [Url] public string Github = string.Empty;

        [Url] public string Youtube = string.Empty;

        [Url] public string BiliBili = string.Empty;
    }

    public SocialLinksInput SocialLinksForm { get; set; } = new SocialLinksInput();
}