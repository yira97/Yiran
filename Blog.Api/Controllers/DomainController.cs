using Blog.Domain.Models;
using Blog.Api.Services;
using Blog.Domain.Enums;
using Blog.Domain.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[Authorize(Policy = Policy.RequireAdmin)]
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

    /// <summary>
    /// 获取所有的 Domain
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomainDto>>> List()
    {
        var data = await _unitOfWork.PostRepository.ListAllDomains();
        return Ok(data);
    }

    /// <summary>
    /// 获取单个 Domain
    /// </summary>
    /// <param name="domainId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{domainId}")]
    public async Task<ActionResult<DomainDto>> Get(string domainId)
    {
        var data = await _unitOfWork.PostRepository.GetDomainInfo(domainId);
        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    /// <summary>
    /// 创建 Domain
    /// </summary>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<DomainDto>> Create(DomainUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var result = _unitOfWork.PostRepository.CreateDomain(updateDto, userId);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(Get), new { domainId = result.Id }, result);
    }

    /// <summary>
    /// 更新 Domain
    /// </summary>
    /// <param name="domainId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{domainId}")]
    public async Task<ActionResult<DomainDto>> Update(string domainId, DomainUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var result = await _unitOfWork.PostRepository.UpdateDomain(domainId, updateDto, userId);
        await _unitOfWork.CompleteAsync();

        return Ok(result);
    }

    /// <summary>
    /// 删除 Domain
    /// </summary>
    /// <param name="domainId"></param>
    /// <returns></returns>
    [HttpDelete("{domainId}")]
    public async Task<ActionResult> Delete(string domainId)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.PostRepository.DeleteDomain(domainId, userId);
        await _unitOfWork.CompleteAsync();

        return Ok();
    }

    [HttpPost("{domainId}/categories")]
    public async Task<ActionResult<DomainCategoryDto>> AddCategory(string domainId, DomainCategoryUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var category = await _unitOfWork.PostRepository.AddDomainCategory(domainId: domainId, updateDto: updateDto,
            userId: userId);
        await _unitOfWork.CompleteAsync();

        return Ok(category);
    }

    [HttpPost("{domainId}/topics")]
    public async Task<ActionResult<DomainCategoryDto>> AddTopic(string domainId, DomainTopicUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var topic = await _unitOfWork.PostRepository.AddDomainTopic(domainId: domainId, updateDto: updateDto,
            userId: userId);
        await _unitOfWork.CompleteAsync();

        return Ok(topic);
    }

    [HttpPut("{domainId}/categories/{categoryId}")]
    public async Task<ActionResult<DomainCategoryDto>> UpdateCategory(string domainId, string categoryId,
        DomainCategoryUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var category = await _unitOfWork.PostRepository.UpdateDomainCategory(domainCategoryId: categoryId,
            updateDto: updateDto, userId: userId);
        await _unitOfWork.CompleteAsync();

        return Ok(category);
    }

    [HttpPut("{domainId}/topics/{topicId}")]
    public async Task<ActionResult<DomainTopicDto>> UpdateTopic(string domainId, string topicId,
        DomainTopicUpdateDto updateDto)
    {
        var userId = HttpContext.User.GetUserId();
        var category = await _unitOfWork.PostRepository.UpdateDomainTopic(domainTopicId: topicId,
            updateDto: updateDto, userId: userId);
        await _unitOfWork.CompleteAsync();

        return Ok(category);
    }

    [HttpDelete("{domainId}/categories/{categoryId}")]
    public async Task<ActionResult> DeleteCategory(string categoryId, string domainId)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.PostRepository.DeleteDomainCategoryImmediately(categoryId, userId);

        return Ok();
    }

    [HttpDelete("{domainId}/topics/{topicId}")]
    public async Task<ActionResult> DeleteTopic(string topicId, string domainId)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.PostRepository.DeleteDomainTopicImmediately(topicId, userId);

        return Ok();
    }

    [HttpPut("{domainId}/site-map")]
    public async Task<ActionResult> UpdateSiteMap(string domainId, SiteMapDto siteMap)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.DomainRepository.UpdateSiteMap(domainId, siteMap, userId);
        await _unitOfWork.CompleteAsync();
        return Ok();
    }

    [HttpPut("{domainId}/site-map/translation/{language}")]
    public async Task<ActionResult> UpdateSiteMapTranslation(string domainId, string language, SiteMapDto siteMap)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.DomainRepository.UpdateSiteMapTranslation(domainId, siteMap, userId);
        await _unitOfWork.CompleteAsync();
        return Ok();
    }

    [HttpPut("{domainId}/social-links")]
    public async Task<ActionResult> UpdateSocialLinks(string domainId, SocialLinksDto socialLinks)
    {
        var userId = HttpContext.User.GetUserId();
        _ = await _unitOfWork.DomainRepository.UpdateSocialLinks(domainId, socialLinks, userId);
        await _unitOfWork.CompleteAsync();
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("{domainId}/social-links")]
    public async Task<ActionResult<SocialLinksDto>> GetSocialLinks(string domainId)
    {
        var socialLinks = await _unitOfWork.DomainRepository.GetSocialLinks(domainId);
        return Ok(socialLinks);
    }

    [AllowAnonymous]
    [HttpGet("{domainId}/site-map/{language?}")]
    public async Task<ActionResult<SiteMapDto>> GetSiteMap(string domainId, string? language)
    {
        SiteMapDto? siteMap;
        if (language == null)
        {
            siteMap = await _unitOfWork.DomainRepository.GetSiteMap(domainId);
        }
        else
        {
            siteMap = await _unitOfWork.DomainRepository.GetSiteMap(domainId, language);
        }

        if (siteMap == null) return NotFound();
        return Ok(siteMap);
    }
}