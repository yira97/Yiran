using Blog.Domain.Models;

namespace Blog.Api.Entities;

public static class DomainTopicEntityExtensions
{
    public static DomainTopicDto DomainTopicDto(this DomainTopicEntity src)
    {
        var dto = new DomainTopicDto(
            Id: src.Id,
            Name: src.Name
        );

        return dto;
    }
}