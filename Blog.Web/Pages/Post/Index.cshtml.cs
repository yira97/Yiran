using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Blog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Post;

public class Index : PageModel
{
    private readonly BlogService _blogService;
    private readonly IDomainService _domainService;

    public Index(BlogService blogService, IDomainService domainService)
    {
        _blogService = blogService;
        _domainService = domainService;
    }

    [BindProperty(SupportsGet = true)] public string PostId { get; set; } = string.Empty;

    public PostDto Post { get; set; }
    public DomainTopicDto TopicInfo { get; set; }
    public DomainCategoryDto CategoryInfo { get; set; }


    public async Task OnGet()
    {
        var domainInfo = await _domainService.GetInfo();
        Post = await _blogService.GetPost(PostId);
        TopicInfo = domainInfo.Topics.First(t => t.Id == Post.Topic);
        CategoryInfo = domainInfo.Categories.First(c => c.Id == Post.Category);
    }
}