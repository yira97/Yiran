using Blog.Domain.Models;
using Blog.Domain.Services.Client;

namespace Blog.Web.Services;

public class DomainService : IDomainService
{
    private readonly BlogService _blogService;
    private string DomainId { get; set; }
    private DomainDto? DomainInfo { get; set; }

    public DomainService(BlogService blogService, IConfiguration configuration)
    {
        _blogService = blogService;
        DomainId = configuration.GetSection("Domain").Get<string>()!;
    }

    private async Task<DomainDto> UpdateDomainInfo()
    {
        var info = await _blogService.GetDomainAsync(DomainId);
        DomainInfo = info;
        return DomainInfo;
    }

    public async Task<DomainDto> GetInfo()
    {
        if (DomainInfo != null) return DomainInfo;
        var domainInfo = await UpdateDomainInfo();
        return domainInfo;
    }
}