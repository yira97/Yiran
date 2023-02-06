using System.Collections;
using Blog.Domain.Enums;
using Blog.Domain.Models;

namespace Blog.Api.Entities;

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

    public static IEnumerable<CategoryResource> CollectStaticResource(this PostDto src)
    {
        var srcElements = new List<CategoryResource>();

        // 封面
        if (src.Content.Cover?.ResourceId is { Length: > 0 } cover)
        {
            _ = srcElements.Append(new CategoryResource(ResourceId: cover,
                Category: (int)StaticResourceCategory.POST_COVER));
        }

        // 内容
        foreach (var b in src.Content.Blocks)
        {
            if (b.Images != null)
            {
                foreach (var im in b.Images)
                {
                    if (im.ResourceId.Length == 0) throw new BadHttpRequestException("no resource id in content");
                    _ = srcElements.Append(new CategoryResource(
                        ResourceId: im.ResourceId,
                        Category: (int)StaticResourceCategory.POST_COVER)
                    );
                }
            }
        }

        return srcElements;
    }

    public static StaticResourceRelatedResult<bool> CompareStaticResource(this PostDto src, PostDto target)
    {
        var srcElements = src.CollectStaticResource().ToList();
        var targetElements = target.CollectStaticResource().ToList();

        var removeElements = srcElements.Except(targetElements).ToList();
        var addElements = targetElements.Except(srcElements).ToList();


        return new StaticResourceRelatedResult<bool>
        {
            Data = !addElements.Except(removeElements).Any(),
            Added = addElements,
            Removed = removeElements.Select(e => e.ResourceId).ToList(),
        };
    }
}