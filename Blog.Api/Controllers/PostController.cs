using Blog.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;

    public PostController(ILogger<PostController> logger)
    {
        _logger = logger;
    }

    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<PostDto>>> List(string domainId, int pageSize, int pageToken, int orderBy, int ascending, )
    // {
    //     
    // }
    //
    // [HttpGet(Name = "{postId}")]
    // public Task<PostDto> Get(string postId)
    // {
    //     
    // }
    //
    // [HttpPost]
    // public Task<PostDto> Create()
    // {
    //     
    // }
    //
    // [HttpPut]
    // public Task<PostDto> Update()
    // {
    //     
    // }
    //
    // [HttpDelete]
    // public Task Delete()
    // {
    //     
    // } 
}