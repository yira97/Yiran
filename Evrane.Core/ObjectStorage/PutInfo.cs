namespace Evrane.Core.ObjectStorage;

public record PutInfo
(
    string Url,
    DateTime ExpiresAt,
    string ResourceId
);