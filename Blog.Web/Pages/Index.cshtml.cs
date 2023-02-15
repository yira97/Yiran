using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly BlogService _blogService;
    private readonly IDomainService _domainService;

    [BindProperty(SupportsGet = true)] public string? TopicId { get; set; }

    public List<PostDto> Posts { get; set; } = new();
    public List<DomainCategoryDto> Categories { get; set; } = new();
    public List<DomainTopicDto> Topics { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, BlogService blogService, IDomainService domainService)
    {
        _logger = logger;
        _blogService = blogService;
        _domainService = domainService;
    }

    public async Task OnGet()
    {
        var domainInfo = await _domainService.GetDomainInfo();

        const int pageSize = 10;
        var pageToken = string.Empty;
        const int orderBy = 0;
        const bool ascending = false;
        var domainId = domainInfo.Id;
        const bool publicOnly = true;

        var result = await _blogService.ListPosts(
            pageSize: pageSize,
            pageToken: pageToken,
            orderBy: orderBy,
            ascending: ascending,
            domainId: domainInfo.Id,
            publicOnly: publicOnly,
            categoryId: null,
            topicId: TopicId
        );
        Posts.AddRange(result.Data);

        Categories = domainInfo.Categories.ToList();
        Topics = domainInfo.Topics.ToList();
        ViewData["CurrentTopic"] = TopicId;
    }
}