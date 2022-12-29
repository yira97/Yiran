using Blog.Api.Enums;
using Blog.Api.Models;

namespace Blog.Api.Repositories;

public interface IStaticResourceRepository
{
    StaticResourceDto Create(StaticResourceUpdateDto updateDto, string userId);

    Task<StaticResourceDto> Categorize(string resourceId, StaticResourceCategory category, string referenceId,
        string userId);
}