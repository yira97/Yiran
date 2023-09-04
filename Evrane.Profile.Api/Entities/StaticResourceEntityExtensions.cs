using Evrane.Profile.Domain.Constants;
using Evrane.Profile.Domain.Models;

namespace Evrane.Profile.Api.Entities;

public static class StaticResourceEntityExtensions
{
    public static StaticResourceDto StaticResourceDto(this StaticResourceEntity src)
    {
        var dto = new StaticResourceDto(src.Id);
        return dto;
    }

    public static StaticResourceArchiveDto StaticResourceArchiveDto(this StaticResourceEntity src)
    {
        var dto = new StaticResourceArchiveDto(
            Id: src.Id,
            Key: src.Key,
            Category: src.Category,
            ReferenceId: src.ReferenceId,
            Action: src.Action
        );

        return dto;
    }
}