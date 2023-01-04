using Blog.Domain.Enums;
using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IStaticResourceRepository
{
    StaticResourceArchiveDto Create(StaticResourceUpdateDto updateDto, string userId);

    Task<StaticResourceDto> Categorize(string resourceId, StaticResourceCategory category, string referenceId,
        string userId);

    Task Categorize(IEnumerable<string> resourceIds, StaticResourceCategory category, string referenceId,
        string userId);

    Task Unlink(string resourceId, string userId);

    Task Unlink(IEnumerable<string> resourceIds, string userId);

    Task<StaticResourceArchiveDto> Get(string resourceId);

    Task<string> GetKey(string resourceId);
}