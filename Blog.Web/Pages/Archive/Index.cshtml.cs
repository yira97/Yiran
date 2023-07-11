using System.Collections.Specialized;
using System.Globalization;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Blog.Web.Pages.Archive;

public class Index : PageModel
{
    private readonly IDomainService _domainService;
    private readonly BlogService _blogService;
    private readonly IStringLocalizer<Index> _localizer;
    private readonly ILogger<Index> _logger;

    public readonly int PageSize = 10;

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

    public SortedDictionary<DateTime, List<PostDto>> PostForDisplay { get; set; } = new();
    public List<DomainCategoryDto> Categories { get; set; } = new();
    public List<DomainTopicDto> Topics { get; set; } = new();

    public string? PreviousPageToken { get; set; } = null;
    public string? NextPageToken { get; set; } = null;

    public Index(IDomainService domainService, BlogService blogService, IStringLocalizer<Index> localizer, ILogger<Index> logger)
    {
        _domainService = domainService;
        _blogService = blogService;
        _localizer = localizer;
        _logger = logger;

        DefaultTopicSelectionDisplay = _localizer["所有话题"];
        DefaultYearSelectionDisplay = _localizer["全部年份"];
        DefaultMonthSelectionDisplay = _localizer["全部月份"];
    }

    public async Task OnGet(string? pageToken = null)
    {
        var domainInfo = await _domainService.GetDomainInfo();

        var parsedPageToken = string.IsNullOrEmpty(pageToken) ? string.Empty : pageToken;
        const int orderBy = 0;
        const bool ascending = false;
        var domainId = domainInfo.Id;
        const bool publicOnly = true;
        var topicId = TopicIdFilter == "all" ? null : TopicIdFilter;

        Posts = await _blogService.ListPosts(PageSize, parsedPageToken, orderBy, ascending, domainId, publicOnly, null,
            topicId);

        Posts.Data.ForEach(p =>
        {
            var time = new DateTime(year: p.CreatedAt.Year, month: p.CreatedAt.Month, day: 1);
            if (PostForDisplay.TryGetValue(time, out var postsInTime))
            {
                postsInTime.Add(p);
            }
            else
            {
                postsInTime = new List<PostDto>()
                {
                    p
                };
            }

            PostForDisplay[time] = postsInTime;
        });

        Categories = domainInfo.Categories.ToList();
        Topics = domainInfo.Topics.ToList();

        TopicSelectionDisplay = Topics.Find(t => t.Id == topicId)?.Name ?? DefaultTopicSelectionDisplay;
        YearSelectionDisplay = YearFilter == null ? DefaultYearSelectionDisplay : YearFilter.ToString()!;
        MonthSelectionDisplay = MonthFilter == null ? DefaultMonthSelectionDisplay : MonthFilter.ToString()!;
        
        // log for parameter
        _logger.LogDebug("TopicSelectionDisplay: {TopicSelectionDisplay}, YearSelectionDisplay: {YearSelectionDisplay}, MonthSelectionDisplay: {MonthSelectionDisplay}, PageToken: {PageToken}", TopicSelectionDisplay, YearSelectionDisplay, MonthSelectionDisplay, pageToken);

        // log the posts 
        _logger.LogDebug("Posts: {Posts}", Posts);
        
        PreviousPageToken = Posts.PreviousPage;

        if (Posts.HasNext)
        {
            NextPageToken = Posts.NextPage;
        }
        else
        {
            NextPageToken = null;
        }
        
        if (pageToken == null || !Posts.HasPrevious)
        {
            PreviousPageToken = null;
        }
        else
        {
            PreviousPageToken = Posts.PreviousPage;
        }
        
        _logger.LogDebug("PreviousPageToken: {PreviousPageToken}, NextPageToken: {NextPageToken}", PreviousPageToken, NextPageToken);
    }
}