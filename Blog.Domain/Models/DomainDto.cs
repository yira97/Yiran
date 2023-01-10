namespace Blog.Domain.Models;

public record DomainDto(
    string Id,
    string Name,
    IEnumerable<DomainCategoryDto> Categories,
    IEnumerable<DomainTopicDto> Topics
);