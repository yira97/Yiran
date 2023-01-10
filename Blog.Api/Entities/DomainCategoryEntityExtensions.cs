using Blog.Domain.Models;

namespace Blog.Api.Entities;

public static class DomainCategoryEntityExtensions
{
    public static DomainCategoryDto DomainCategoryDto(this DomainCategoryEntity src)
    {
        var dto = new DomainCategoryDto(
            Id: src.Id,
            Name: src.Name
        );

        return dto;
    }
}