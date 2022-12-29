namespace Blog.Api.Models;

public record StaticResourceUpdateDto
(
        int Action,
        long FileSzie,
        // "_.png" 至少存在扩展名
        string OriginalFileName
);