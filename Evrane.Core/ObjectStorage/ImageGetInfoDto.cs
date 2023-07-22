namespace Evrane.Core.ObjectStorage;

public record ImageGetInfoDto
(
    string Url,
    string UrlXs,
    string UrlSm,
    string UrlMd,
    string UrlLg,
    string UrlXl,
    DateTime ExpiresAt,
    string ResourceId,
    string? UrlXsCrop169 = "",
    string? UrlSmCrop169 = "",
    string? UrlMdCrop169 = "",
    string? UrlLgCrop169 = "",
    string UrlXlCrop169 = ""
) : GetInfo(Url, ExpiresAt, ResourceId);