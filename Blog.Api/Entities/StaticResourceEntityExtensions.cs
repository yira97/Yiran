using System.Collections;
using Blog.Api.Enums;
using Blog.Api.Models;

namespace Blog.Api.Entities;

public static class StaticResourceEntityExtensions
{
    public static StaticResourceDto StaticResourceDto(this StaticResourceEntity src)
    {
        var dto = new StaticResourceDto(src.Id);
        return dto;
    }

    public static IEnumerable<CategoryResource> CollectStaticResource(this PostDto src)
    {
        var srcElements = new List<CategoryResource>();

        if (src.Content.Cover?.ResourceId is { Length: > 0 } cover)
        {
            _ = srcElements.Append(new CategoryResource(ResourceId: cover,
                Category: (int)StaticResourceCategory.POST_COVER));
        }

        foreach (var b in src.Content.Blocks)
        {
            if (b.Image != null)
            {
                if (b.Image.ResourceId.Length > 0) throw new BadHttpRequestException("no resource id in cover");
                _ = srcElements.Append(new CategoryResource(ResourceId: b.Image.ResourceId,
                    Category: (int)StaticResourceCategory.POST_COVER));
            }
            else if (b.Images != null)
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