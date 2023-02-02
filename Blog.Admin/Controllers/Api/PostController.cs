using Blog.Admin.Middlewares;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly BlogService _blogService;

    public PostController(BlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet("{id}/post-content", Name = "GetPostContent")]
    public async Task<ActionResult<PostContentDto>> GetPostContent(string id)
    {
        var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
        var post = await _blogService.GetPost(id, accessToken.AccessToken);
        return post.Content;
    }
}