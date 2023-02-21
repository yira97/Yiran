using System.Text.Json;
using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Domain.Enums;
using Blog.Domain.Models;
using Evrane.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Repositories;

public class DomainRepository : IDomainRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DomainRepository> _logger;

    public DomainRepository(ApplicationDbContext context, ILogger<DomainRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 并不填充所有topic和category的字段
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<DomainDto>> ListAllDomains()
    {
        var query = _context.Domains.Where(d => d.DeletedAt == null).Take(20).AsQueryable();

        var result = await query.ToListAsync();

        return result.Select(r => r.DomainDto()).ToList();
    }

    public DomainDto CreateDomain(DomainUpdateDto updateDto, string userId)
    {
        var domainSiteMap = new SiteMapEntity();
        _context.Add(domainSiteMap);

        var model = new DomainEntity();

        model.Name = updateDto.Name;
        model.SiteMapId = domainSiteMap.Id;

        model.UpdatedById = userId;
        model.CreatedById = userId;

        _context.Add(model);

        return model.DomainDto();
    }

    private async Task<DomainEntity?> GetDomainOrDefault(string domainId)
    {
        var domain = await _context.Domains
            .Include(d => d.Categories)
            .Include(d => d.Topics)
            .Where(d => d.Id == domainId)
            .FirstOrDefaultAsync();

        if (domain == null || domain.DeletedAt != null)
        {
            return null;
        }

        return domain;
    }

    private async Task<DomainEntity> GetDomain(string domainId)
    {
        var domain = await _context.Domains
            .Include(d => d.Categories)
            .Include(d => d.Topics)
            .Where(d => d.Id == domainId)
            .FirstAsync();

        if (domain == null || domain.DeletedAt != null)
        {
            throw new BadHttpRequestException("id not found");
        }

        return domain;
    }

    public async Task<DomainDto?> GetDomainInfo(string domainId)
    {
        var domain = await GetDomainOrDefault(domainId);
        return domain?.DomainDto();
    }

    public async Task<DomainDto?> GetDomainInfoByName(string domainName)
    {
        var domain = await _context.Domains.Where(d => d.Name == domainName && d.DeletedAt == null)
            .FirstOrDefaultAsync();
        return domain?.DomainDto();
    }

    public async Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto, string userId)
    {
        var domain = await GetDomain(domainId);

        domain.Name = updateDto.Name;

        domain.UpdatedAt = DateTime.UtcNow;
        domain.UpdatedById = userId;

        return domain.DomainDto();
    }

    public async Task<bool> DeleteDomain(string domainId, string userId)
    {
        var domain = await _context.Domains.FindAsync(domainId);
        if (domain == null || domain.DeletedAt != null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        domain.UpdatedById = userId;
        domain.UpdatedAt = now;
        domain.DeletedAt = now;

        _logger.LogInformation("delete domain");

        return true;
    }

    public async Task<DomainCategoryDto> AddDomainCategory(string domainId, DomainCategoryUpdateDto updateDto,
        string userId)
    {
        if (await _context.DomainCategories.Where(dc => dc.DomainId == domainId && dc.Name == updateDto.Name)
                .AnyAsync())
        {
            throw new EvraneException(EvraneStatusCode.BadRequest, "category already exists");
        }

        var category = new DomainCategoryEntity
        {
            Name = updateDto.Name,
            DomainId = domainId,
            UpdatedById = userId,
            CreatedById = userId,
        };

        _context.Add(category);

        return category.DomainCategoryDto();
    }

    public async Task<DomainCategoryDto> UpdateDomainCategory(string domainCategoryId,
        DomainCategoryUpdateDto updateDto, string userId)
    {
        var category = await _context.DomainCategories.FindAsync(domainCategoryId);
        if (category == null)
        {
            throw new EvraneException(EvraneStatusCode.ResourceNotFound, "category not exists");
        }

        category.Name = updateDto.Name;
        category.UpdatedById = userId;
        category.UpdatedAt = DateTime.UtcNow;

        return category.DomainCategoryDto();
    }

    public async Task<bool> DeleteDomainCategoryImmediately(string domainCategoryId, string userId)
    {
        var category = await _context.DomainCategories.FindAsync(domainCategoryId);
        if (category == null)
        {
            return false;
        }

        _context.Remove(category);

        await _context.SaveChangesAsync();

        await _context.Posts
            .Where(p => p.DomainId == category.DomainId && p.Category == category.Name)
            .ExecuteUpdateAsync(spc => spc.SetProperty(p => p.Category, c => ""));

        return true;
    }

    public async Task<DomainTopicDto> AddDomainTopic(string domainId, DomainTopicUpdateDto updateDto, string userId)
    {
        if (await _context.DomainTopics.Where(dc => dc.DomainId == domainId && dc.Name == updateDto.Name)
                .AnyAsync())
        {
            throw new EvraneException(EvraneStatusCode.BadRequest, "topic already exists");
        }

        var category = new DomainTopicEntity()
        {
            Name = updateDto.Name,
            DomainId = domainId,
            UpdatedById = userId,
            CreatedById = userId,
        };

        _context.Add(category);

        return category.DomainTopicDto();
    }

    public async Task<DomainTopicDto> UpdateDomainTopic(string domainTopicId, DomainTopicUpdateDto updateDto,
        string userId)
    {
        var topic = await _context.DomainTopics.FindAsync(domainTopicId);
        if (topic == null)
        {
            throw new EvraneException(EvraneStatusCode.ResourceNotFound, "topic not exists");
        }

        topic.Name = updateDto.Name;
        topic.UpdatedById = userId;
        topic.UpdatedAt = DateTime.UtcNow;

        return topic.DomainTopicDto();
    }

    public async Task<bool> DeleteDomainTopicImmediately(string domainTopicId, string userId)
    {
        var topic = await _context.DomainTopics.FindAsync(domainTopicId);
        if (topic == null)
        {
            return false;
        }

        _context.Remove(topic);

        await _context.SaveChangesAsync();

        await _context.Posts
            .Where(p => p.DomainId == topic.DomainId && p.Topic == topic.Name)
            .ExecuteUpdateAsync(spc => spc.SetProperty(p => p.Category, c => ""));

        return true;
    }

    public async Task<SiteMapDto?> GetSiteMap(string domainId)
    {
        var domain = await _context.Domains.Include(d => d.SiteMap).Where(d => d.Id == domainId && d.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (domain?.SiteMap?.TextContentId == null) return null;
        var siteMapContentText = await _context.TextContents.FindAsync(domain.SiteMap!.TextContentId);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapContentText!.OriginalText, options);
        return siteMap!;
    }

    public async Task<SiteMapDto?> GetSiteMap(string domainId, string language)
    {
        var domain = await _context.Domains.Include(d => d.SiteMap).Where(d => d.Id == domainId && d.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (domain?.SiteMap?.TextContentId == null) return null;
        var siteMapContentText = await _context.TextContents.FindAsync(domain.SiteMap.TextContentId);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        SiteMapDto? siteMap;
        if (siteMapContentText!.Language == language)
        {
            siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapContentText.OriginalText, options);
            return siteMap;
        }

        var siteMapContentTranslation = await _context.TextContentTranslations
            .Where(tct => tct.Language == language && tct.TextContentId == domain.SiteMap.TextContentId)
            .FirstOrDefaultAsync();
        if (siteMapContentTranslation == null) return null;

        siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapContentTranslation.Translation, options);
        return siteMap!;
    }

    public async Task<SiteMapDto> UpdateSiteMap(string domainId, SiteMapDto siteMap, string userId)
    {
        var domain = await _context.Domains.Include(d => d.SiteMap).Where(d => d.Id == domainId && d.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (domain == null) throw new EvraneException(EvraneStatusCode.ResourceNotFound, "domain not exist");
        if (domain.SiteMap == null) throw new EvraneException(EvraneStatusCode.ServerError, "no domain sitemap");
        if (!Language.Languages.Contains(siteMap.Language))
            throw new EvraneException(EvraneStatusCode.BadRequest, "invalid language");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var siteMapText = JsonSerializer.Serialize(siteMap, options);
        TextContentEntity textContent;

        // 有，直接更新
        if (domain.SiteMap.TextContentId != null)
        {
            textContent = (await _context.TextContents.FindAsync(domain.SiteMap.TextContentId))!;
            textContent.Language = siteMap.Language;
            textContent.OriginalText = siteMapText;
        }
        // 无，创建
        else
        {
            textContent = new TextContentEntity
            {
                Language = siteMap.Language,
                OriginalText = siteMapText,
            };
            _context.Add(textContent);

            domain.SiteMap.TextContentId = textContent.Id;
        }

        domain.UpdatedById = userId;
        domain.UpdatedAt = DateTime.UtcNow;
        return siteMap;
    }

    public async Task<SiteMapDto> UpdateSiteMapTranslation(string domainId, SiteMapDto siteMap, string userId)
    {
        var domain = await _context.Domains.Include(d => d.SiteMap).Where(d => d.Id == domainId && d.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (domain == null) throw new EvraneException(EvraneStatusCode.ResourceNotFound, "domain not exist");
        if (domain.SiteMap == null) throw new EvraneException(EvraneStatusCode.ServerError, "no domain sitemap");
        if (!Language.Languages.Contains(siteMap.Language))
            throw new EvraneException(EvraneStatusCode.BadRequest, "invalid language");
        if (domain.SiteMap.TextContentId == null)
            throw new EvraneException(EvraneStatusCode.BadRequest, "you need original language first");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var siteMapText = JsonSerializer.Serialize(siteMap, options);


        var translation = await _context.TextContentTranslations
            .Where(tct => tct.TextContentId == domain.SiteMap.TextContentId && tct.Language == siteMap.Language)
            .FirstOrDefaultAsync();

        // 有，直接更新
        if (translation != null)
        {
            // 无修改直接返回
            if (translation.Translation == siteMapText) return siteMap;

            translation.Translation = siteMapText;
        }
        else
        {
            _context.Add(new TextContentTranslationEntity
            {
                Language = siteMap.Language,
                TextContentId = domain.SiteMap.TextContentId,
                Translation = siteMapText,
            });
        }

        return siteMap;
    }

    public async Task<SocialLinksDto> UpdateSocialLinks(string domainId, SocialLinksDto socialDto, string userId)
    {
        var domain = await _context.Domains.FindAsync(domainId);
        if (domain == null) throw new EvraneException(EvraneStatusCode.ResourceNotFound, "domain not exist");

        domain.SocialLinks = socialDto;
        domain.UpdatedAt = DateTime.UtcNow;
        domain.UpdatedById = userId;

        return socialDto;
    }

    public async Task<SocialLinksDto> GetSocialLinks(string domainId)
    {
        var domain = await _context.Domains.FindAsync(domainId);
        if (domain == null) throw new EvraneException(EvraneStatusCode.ResourceNotFound, "domain not exist");

        return domain.SocialLinks;
    }
}