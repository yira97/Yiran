using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Api.Enums;
using Blog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Repositories;

public class StaticResourceRepository : IStaticResourceRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StaticResourceRepository> _logger;

    public StaticResourceRepository(ApplicationDbContext context, ILogger<StaticResourceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    private const string KeyPrefix = "com/evrane/yiran";

    public StaticResourceArchiveDto Create(StaticResourceUpdateDto updateDto, string userId)
    {
        var model = new StaticResourceEntity();

        // check filename
        if (updateDto.OriginalFileName.IndexOf('.') < 0)
        {
            throw new BadHttpRequestException("invalid OriginalFileName");
        }

        var ext = Path.GetExtension(updateDto.OriginalFileName);
        var newFileName = Guid.NewGuid() + ext;

        model.Action = updateDto.Action;
        var now = DateTime.UtcNow;
        model.Key =
            $"{KeyPrefix}/{model.Action.ToString("0000")}/{userId}/{now.Year.ToString("0000")}/{now.Month.ToString("00")}/{now.Day.ToString("00")}/{now.Hour.ToString("00")}/{newFileName}";
        model.CreatedById = userId;
        model.CreatedAt = now;
        model.UpdatedById = userId;
        model.UpdatedAt = now;
        model.OriginalFileName = updateDto.OriginalFileName;
        model.FileSize = updateDto.FileSize;

        _context.StaticResources.Add(model);

        return model.StaticResourceArchiveDto();
    }

    public async Task<StaticResourceDto> Categorize(string resourceId, StaticResourceCategory category,
        string referenceId,
        string userId)
    {
        var e = await _context.StaticResources.FindAsync(resourceId);
        if (e == null || e.DeletedAt != null) throw new BadHttpRequestException("resource not found");
        if (e.Category != null)
            throw new BadHttpRequestException("resource can not categorize twice");
        e.Category = (int)category;
        e.ReferenceId = referenceId;
        e.UpdatedById = userId;
        e.UpdatedAt = DateTime.UtcNow;
        return e.StaticResourceDto();
    }

    public async Task Categorize(IEnumerable<string> resourceIds, StaticResourceCategory category, string referenceId,
        string userId)
    {
        var resourceIdList = resourceIds.ToList();

        if (resourceIdList.Count > 100)
        {
            foreach (var rId in resourceIdList)
            {
                await Categorize(rId, category, referenceId, userId);
            }
        }

        var rs = await _context.StaticResources.Where(r => resourceIdList.Contains(r.Id)).ToListAsync();
        if (rs.Count != resourceIdList.Count) throw new BadHttpRequestException("id not found");
        if (rs.Any(r => r.DeletedAt != null)) throw new BadHttpRequestException("id not found");
        if (rs.Any(r => r.Category != null)) throw new BadHttpRequestException("resource can not categorize twice");

        foreach (var r in rs)
        {
            r.Category = (int)category;
            r.ReferenceId = referenceId;
            r.UpdatedById = userId;
            r.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task Unlink(string resourceId, string userId)
    {
        var e = await _context.StaticResources.FindAsync(resourceId);
        if (e == null || e.DeletedAt != null) throw new BadHttpRequestException("resource not found");

        e.Category = null;
        e.ReferenceId = null;
        e.UpdatedById = userId;
        e.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///
    /// 
    /// </summary>
    /// <param name="resourceIds"></param>
    /// <param name="userId"></param>
    /// <exception cref="BadHttpRequestException"></exception>
    public async Task Unlink(IEnumerable<string> resourceIds, string userId)
    {
        var resourceIdList = resourceIds.ToList();

        if (resourceIdList.Count > 100)
        {
            foreach (var rId in resourceIdList)
            {
                await Unlink(rId, userId);
            }
        }

        var rs = await _context.StaticResources.Where(r => resourceIdList.Contains(r.Id)).ToListAsync();
        if (rs.Count != resourceIdList.Count) throw new BadHttpRequestException("id not found");
        if (rs.Any(r => r.DeletedAt != null)) throw new BadHttpRequestException("id not found");

        foreach (var r in rs)
        {
            r.Category = null;
            r.ReferenceId = null;
            r.UpdatedById = userId;
            r.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<StaticResourceArchiveDto> Get(string resourceId)
    {
        var e = await _context.StaticResources.FindAsync(resourceId);
        if (e == null || e.DeletedAt != null)
            throw new BadHttpRequestException("resource not found");

        return e.StaticResourceArchiveDto();
    }

    public async Task<string> GetKey(string resourceId)
    {
        var e = await _context.StaticResources.FindAsync(resourceId);
        if (e == null || e.DeletedAt != null) throw new BadHttpRequestException("resource not found");
        return e.Key;
    }
}