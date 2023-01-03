using Blog.Api.Models;
using Blog.Api.Services;
using Blog.Core.ObjectStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class StaticResourceController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStorageService _objectStorageService;


    [AllowAnonymous]
    [HttpGet("{resourceId}")]
    public async Task<ActionResult<GetInfo>> GetTempGetInfo(string resourceId)
    {
        var userId = "Todo";

        var r = await _unitOfWork.StaticResourceRepository.Get(resourceId);

        var info = await _objectStorageService.GetInfo(r.Key, TimeSpan.FromHours(1));

        return info;
    }

    [AllowAnonymous]
    [HttpGet("upload")]
    public async Task<ActionResult<PutInfo>> GetPutInfo(StaticResourceUpdateDto resourceDto)
    {
        var userId = "todo";

        var r = _unitOfWork.StaticResourceRepository.Create(resourceDto, userId);

        await _unitOfWork.CompleteAsync();

        var info = await _objectStorageService.PutInfo(r.Key, TimeSpan.FromHours(12));

        await _unitOfWork.CompleteAsync();

        return info;
    }
}