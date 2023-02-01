using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Archive;

public class Index : PageModel
{
    private readonly IDomainService _domainService;
    private readonly BlogService _blogService;

    [BindProperty(SupportsGet = true)] public string? TopicIdFilter { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearFilter { get; set; }
    [BindProperty(SupportsGet = true)] public int? MonthFilter { get; set; }

    public string TopicSelectionDisplay { get; set; } = string.Empty;
    public string YearSelectionDisplay { get; set; } = string.Empty;
    public string MonthSelectionDisplay { get; set; } = string.Empty;

    public string DefaultTopicSelectionDisplay { get; set; } = "All Topics";
    public string DefaultYearSelectionDisplay { get; set; } = "All Years";
    public string DefaultMonthSelectionDisplay { get; set; } = "All Months";

    public int CurrentYear { get; set; } = DateTime.UtcNow.Year;
    public int YearCount { get; set; } = 5;

    public CursorBasedQueryResult<PostDto> Posts { get; set; } = new();
    public List<DomainCategoryDto> Categories { get; set; } = new();
    public List<DomainTopicDto> Topics { get; set; } = new();

    public Index(IDomainService domainService, BlogService blogService)
    {
        _domainService = domainService;
        _blogService = blogService;
    }

    public async Task OnGet()
    {
        var domainInfo = await _domainService.GetInfo();

        const int pageSize = 10;
        var pageToken = Posts.NextPage;
        const int orderBy = 0;
        const bool ascending = false;
        var domainId = domainInfo.Id;
        const bool publicOnly = true;
        var topicId = TopicIdFilter == "all" ? null : TopicIdFilter;

        Posts = await _blogService.ListPosts(pageSize, pageToken, orderBy, ascending, domainId, publicOnly, null,
            topicId);
        Categories = domainInfo.Categories.ToList();
        Topics = domainInfo.Topics.ToList();

        TopicSelectionDisplay = Topics.Find(t => t.Id == topicId)?.Name ?? DefaultTopicSelectionDisplay;
        YearSelectionDisplay = YearFilter == null ? DefaultYearSelectionDisplay : YearFilter.ToString()!;
        MonthSelectionDisplay = MonthFilter == null ? DefaultMonthSelectionDisplay : MonthFilter.ToString()!;
    }
}