namespace Blog.Core.ObjectStorage;

public record ImageGetInfoDto
(
    string Url,
    string UrlXs,
    string UrlSm,
    string UrlMd,
    string UrlLg,
    string UrlXl,
    DateTime ExpiresAt
) : GetInfo(Url, ExpiresAt);