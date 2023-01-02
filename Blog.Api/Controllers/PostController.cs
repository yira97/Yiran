using Blog.Api.Enums;
using Blog.Api.Models;
using Blog.Api.Services;
using Blog.Core.ObjectStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStorageService _objectStorageService;

    public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork,
        IObjectStorageService objectStorageService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _objectStorageService = objectStorageService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<CursorBasedQueryResult<PostDto>>> List(
        int pageSize,
        string pageToken,
        int orderBy,
        bool ascending,
        [FromQuery(Name = "filter")] Dictionary<int, string> filter
    )
    {
        var parsedFilter = new Dictionary<int, List<string>>();

        foreach (var (key, value) in filter)
        {
            parsedFilter[key] = value.Split(',').ToList();
        }

        var query = new CursorBasedQuery
        {
            PageSize = pageSize,
            PageToken = pageToken,
            OrderBy = orderBy,
            Ascending = ascending,
            Filter = parsedFilter,
        };

        var list = await _unitOfWork.PostRepository.ListPosts(query);

        for (var i = 0; i < list.Data.Count; i++)
        {
            list.Data[i] = await list.Data[i].WithResourceGetInfo(async id =>
            {
                var key = await _unitOfWork.StaticResourceRepository.GetKey(id);
                var info = await _objectStorageService.ImageGetInfo(key, TimeSpan.FromMinutes(10));
                return info;
            });
        }

        return Ok(list);
    }

    [AllowAnonymous]
    [HttpGet(Name = "{postId}")]
    public async Task<ActionResult<PostDto>> Get(string postId)
    {
        var post = await _unitOfWork.PostRepository.GetPostInfo(postId);
        if (post == null) return NotFound();

        post = await post.WithResourceGetInfo(async id =>
        {
            var key = await _unitOfWork.StaticResourceRepository.GetKey(id);
            var info = await _objectStorageService.ImageGetInfo(key, TimeSpan.FromMinutes(10));
            return info;
        });

        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create(PostUpdateDto updateDto)
    {
        var userId = "TODO";
        var result = _unitOfWork.PostRepository.CreatePost(updateDto, userId);
        foreach (var (resourceId, category) in result.Added)
        {
            _ = await _unitOfWork.StaticResourceRepository.Categorize(resourceId, (StaticResourceCategory)category,
                result.Data.Id, userId);
        }

        await _unitOfWork.CompleteAsync();

        result.Data = await result.Data.WithResourceGetInfo(async id =>
        {
            var key = await _unitOfWork.StaticResourceRepository.GetKey(id);
            var info = await _objectStorageService.ImageGetInfo(key, TimeSpan.FromMinutes(10));
            return info;
        });

        return Ok(result.Data);
    }

    [HttpPut(Name = "{postId}")]
    public async Task<ActionResult<PostDto>> Update(PostUpdateDto updateDto, string postId)
    {
        var userId = "TODO";
        var result = await _unitOfWork.PostRepository.UpdatePost(postId, updateDto, userId);

        foreach (var (resourceId, category) in result.Added)
        {
            _ = await _unitOfWork.StaticResourceRepository.Categorize(resourceId, (StaticResourceCategory)category,
                result.Data.Id, userId);
        }

        await _unitOfWork.StaticResourceRepository.Unlink(result.Removed, userId);

        await _unitOfWork.CompleteAsync();

        result.Data = await result.Data.WithResourceGetInfo(async id =>
        {
            var key = await _unitOfWork.StaticResourceRepository.GetKey(id);
            var info = await _objectStorageService.ImageGetInfo(key, TimeSpan.FromMinutes(10));
            return info;
        });

        return Ok(result.Data);
    }

    [HttpDelete(Name = "{postId}")]
    public async Task<ActionResult> Delete(string postId)
    {
        var userId = "TODO";
        _ = await _unitOfWork.PostRepository.DeletePost(postId, userId);
        return Ok();
    }
}