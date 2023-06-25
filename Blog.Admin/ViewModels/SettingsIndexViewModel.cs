using System.ComponentModel.DataAnnotations;
using Blog.Domain.Models;

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

        [Url] public string Facebook { get; set; } = string.Empty;

        [Url] public string LinkedIn { get; set; } = string.Empty;

        [Url] public string Instagram { get; set; } = string.Empty;

        [Url] public string Twitter { get; set; } = string.Empty;

        [Url] public string Github { get; set; } = string.Empty;

        [Url] public string Youtube { get; set; } = string.Empty;

        [Url] public string BiliBili { get; set; } = string.Empty;
    }

    public SocialLinksInput SocialLinksForm { get; set; } = new SocialLinksInput();
}