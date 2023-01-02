namespace Blog.Api.Models;

public record StaticResourceArchiveDto(
    string Id,
    string Key,
    int? Category,
    string? ReferenceId,
    int Action
);