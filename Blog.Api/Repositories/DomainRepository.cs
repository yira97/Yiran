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

    public DomainRepository(ApplicationDbContext context)
    {
        _context = context;
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
            siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapContentText!.OriginalText, options);
            return siteMap;
        }

        var siteMapContentTranslation = await _context.TextContentTranslations
            .Where(tct => tct.Language == language && tct.TextContentId == domain.SiteMap.TextContentId)
            .FirstOrDefaultAsync();
        if (siteMapContentTranslation == null) return null;

        siteMap = JsonSerializer.Deserialize<SiteMapDto>(siteMapContentTranslation!.Translation, options);
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