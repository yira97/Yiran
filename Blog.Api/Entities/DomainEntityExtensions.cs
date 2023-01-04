using Blog.Domain.Models;

namespace Blog.Api.Entities;

public static class DomainEntityExtensions
{
    public static DomainDto DomainDto(this DomainEntity src)
    {
        var dto = new DomainDto(src.Id, src.Name);
        return dto;
    }
}