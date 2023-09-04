namespace Evrane.Profile.Domain.Models;

public record StaticResourceDto
(
    string Id,
    string? Url = null,
    DynamicSizeUrl? DynamicSizeUrl = null
);