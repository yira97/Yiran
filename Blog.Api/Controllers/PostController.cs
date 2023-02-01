using Blog.Domain.Enums;
using Blog.Domain.Models;
using Blog.Api.Services;
using Blog.Domain.Extensions;
using Evrane.Core.ObjectStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[Authorize]
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

    /// <summary>
    /// 列出 Post
    /// </summary>
    /// <param name="pageSize">分页大小</param>
    /// <param name="pageToken">分页Token</param>
    /// <param name="orderBy">排序</param>
    /// <param name="ascending">是否升序</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<CursorBasedQueryResult<PostDto>>> List(
        int pageSize = 5,
        int orderBy = 0,
        bool ascending = false,
        string pageToken = "",
        string? domainId = null,
        bool? publicOnly = null,
        string? categoryId = null,
        string? topicId = null
    )
    {
        var parsedFilter = new Dictionary<int, List<string>>();
        if (domainId != null)
        {
            parsedFilter[(int)PostFilterKey.DomainId] = new() { domainId };
        }

        if (publicOnly != null)
        {
            parsedFilter[(int)PostFilterKey.PublicOnly] = new() { publicOnly.ToString() };
        }

        if (categoryId != null)
        {
            parsedFilter[(int)PostFilterKey.Category] = new() { categoryId };
        }

        if (topicId != null)
        {
            parsedFilter[(int)PostFilterKey.Topic] = new() { topicId };
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

    /// <summary>
    /// 获取单个 Post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDto>> Get(string postId)
    {
        var post = await _unitOfWork.PostRepository.GetPostInfo(postId);
        if (post == null) return NotFound();
        if (!post.isPublic && !HttpContext.User.IsAdmin())
        {
            return Unauthorized();
        }

        post = await post.WithResourceGetInfo(async id =>
        {
            var key = await _unitOfWork.StaticResourceRepository.GetKey(id);
            var info = await _objectStorageService.ImageGetInfo(key, TimeSpan.FromMinutes(10));
            return info;
        });

        return Ok(post);
    }

    /// <summary>
    /// 创建 Post
    /// </summary>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [Authorize(Policy = Policy.RequireAdmin)]
    [HttpPost]
    public async Task<ActionResult<PostDto>> Create(PostUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
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

        return CreatedAtAction(nameof(Get), new { postId = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// 更新 Post
    /// </summary>
    /// <param name="updateDto"></param>
    /// <param name="postId"></param>
    /// <returns></returns>
    [Authorize(Policy = Policy.RequireAdmin)]
    [HttpPut("{postId}")]
    public async Task<ActionResult<PostDto>> Update(PostUpdateDto updateDto, string postId)
    {
        var userId = HttpContext.User.GetUserId();
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

    /// <summary>
    /// 删除 Post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    [Authorize(Policy = Policy.RequireAdmin)]
    [HttpDelete("{postId}")]
    public async Task<ActionResult> Delete(string postId)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.PostRepository.DeletePost(postId, userId);
        return Ok();
    }
}