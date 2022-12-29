namespace Blog.Api.Models;

public record StaticResourceDto
(
    string Id,
    string? Url = null,
    DynamicSizeUrl? DynamicSizeUrl = null
);