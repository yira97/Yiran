using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Api.Enums;
using Blog.Api.Models;

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
    
    public StaticResourceDto Create(StaticResourceUpdateDto updateDto, string userId)
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
        model.FileSize = updateDto.FileSzie;

        _context.StaticResources.Add(model);

        return model.StaticResourceDto();
    }
    
    public async Task<StaticResourceDto> Categorize(string resourceId, StaticResourceCategory category, string referenceId,
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
}