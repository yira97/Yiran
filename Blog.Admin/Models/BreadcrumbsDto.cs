using Blog.Domain.Models;

namespace Blog.Admin.Models;

public record BreadcrumbsDto(
    IEnumerable<NavigationDto> Links
);