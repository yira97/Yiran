using Blog.Api.Models;
using Blog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class DomainController : ControllerBase
{
    private readonly ILogger<DomainController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DomainController(ILogger<DomainController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomainDto>>> List()
    {
        var data =  await _unitOfWork.PostRepository.ListAllDomains();
        return Ok(data);
    }
    
    [HttpGet(Name = "{domainId}")]
    public async Task<ActionResult<DomainDto>> Get(string domainId)
    {
        var data = await _unitOfWork.PostRepository.GetDomainInfo(domainId);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<DomainDto>> Create(DomainUpdateDto updateDto)
    {
        // var userId = HttpContext.User.
        var userId = "TODO";
        var result = _unitOfWork.PostRepository.CreateDomain(updateDto, userId);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(Get), new { domainId = result.Id }, result);
    }

    [HttpPut(Name = "{domainId}")]
    public async Task<ActionResult<DomainDto>> Update(string domainId, DomainUpdateDto updateDto)
    {
        // var userId = HttpContext.User.
        var userId = "TODO";
        var result = await _unitOfWork.PostRepository.UpdateDomain(domainId, updateDto, userId);
        await _unitOfWork.CompleteAsync();

        return Ok(result);
    }

    [HttpDelete(Name = "{domainId}")]
    public async Task<ActionResult> Delete(string domainId)
    {
        // var userId = HttpContext.User.
        var userId = "TODO";
        _ = await _unitOfWork.PostRepository.DeleteDomain(domainId, userId);
        await _unitOfWork.CompleteAsync();

        return Ok();
    } 
}