using Blog.Domain.Enums;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Api;

[Authorize(Policy = Policies.RequireAdminRole)]
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
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var post = await _blogService.GetPost(id);
        return post.Content;
    }
}