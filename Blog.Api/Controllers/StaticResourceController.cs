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
public class StaticResourceController : ControllerBase
{
    private readonly ILogger<StaticResourceController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStorageService _objectStorageService;

    public StaticResourceController(ILogger<StaticResourceController> logger, IUnitOfWork unitOfWork,
        IObjectStorageService objectStorageService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _objectStorageService = objectStorageService;
    }

    [HttpGet("{resourceId}")]
    public async Task<ActionResult<GetInfo>> GetTempGetInfo(string resourceId)
    {
        var userId = HttpContext.User.GetUserId();

        var r = await _unitOfWork.StaticResourceRepository.Get(resourceId);

        var info = await _objectStorageService.GetInfo(r.Key, TimeSpan.FromHours(1));

        return info with { ResourceId = r.Id };
    }

    [HttpPost]
    public async Task<ActionResult<PutInfo>> GetPutInfo(StaticResourceUpdateDto resourceDto)
    {
        var userId = HttpContext.User.GetUserId();

        var r = _unitOfWork.StaticResourceRepository.Create(resourceDto, userId);

        await _unitOfWork.CompleteAsync();

        var info = await _objectStorageService.PutInfo(r.Key, TimeSpan.FromHours(12), new Dictionary<int, string>());

        await _unitOfWork.CompleteAsync();

        return info with { ResourceId = r.Id };
    }
}