namespace Blog.Domain.Models;

public record StaticResourceUpdateDto
(
    int Action,
    long FileSize,
    // "_.png" 至少存在扩展名
    string OriginalFileName
);