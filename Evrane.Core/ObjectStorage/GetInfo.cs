namespace Evrane.Core.ObjectStorage;

public record GetInfo
(
    string Url,
    DateTime ExpiresAt,
    string ResourceId
);