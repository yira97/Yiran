namespace Blog.Core.ObjectStorage;

public record PutInfo
(
    string Url,
    DateTime ExpiresAt
);