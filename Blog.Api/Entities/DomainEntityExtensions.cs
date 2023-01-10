using Blog.Domain.Models;

namespace Blog.Api.Entities;

public static class DomainEntityExtensions
{
    public static DomainDto DomainDto(this DomainEntity src)
    {
        var categories = new List<DomainCategoryDto>();
        if (src is { Categories: { } })
        {
            categories = src.Categories.Select(c => c.DomainCategoryDto()).ToList();
        }

        var topics = new List<DomainTopicDto>();
        if (src is { Topics: { } })
        {
            topics = src.Topics.Select(t => t.DomainTopicDto()).ToList();
        }

        var dto = new DomainDto(src.Id, src.Name, categories, topics);
        return dto;
    }
}