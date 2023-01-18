using Blog.Admin.Models;
using Blog.Domain.Models;

namespace Blog.Admin.Helper;

public static class CookieHelper
{
    private const string AccessTokenKey = "X-Access-Token";
    private const string RefreshTokenKey = "X-Refresh-Token";
    private const string UserEmailKey = "User-Email";
    private const string UserDisplayNameKey = "User-DisplayName";
    private const string DefaultDomainKey = "Default-Domain";
    private const string LanguageKey = "Language";

    public static AccessTokenDto GetAccessTokenFromCookie(HttpContext httpContext)
    {
        var accessToken = httpContext.Request.Cookies[AccessTokenKey];
        var refreshToken = httpContext.Request.Cookies[RefreshTokenKey];

        return new AccessTokenDto(accessToken!, refreshToken!);
    }

    public static void WriteAccessTokenToCookie(HttpContext httpContext, AccessTokenDto accessToken)
    {
        httpContext.Response.Cookies.Append(AccessTokenKey, accessToken.AccessToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        httpContext.Response.Cookies.Append(RefreshTokenKey, accessToken.RefreshToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
    }

    public static string GetLanguageFromCookie(HttpContext httpContext, string defaultValue = "zh")
    {
        var language = httpContext.Request.Cookies[LanguageKey];
        return string.IsNullOrEmpty(language) ? defaultValue : language;
    }

    public static void WriteLanguageToCookie(HttpContext httpContext, string language)
    {
        httpContext.Response.Cookies.Append(LanguageKey, language,
            new CookieOptions());
    }

    public static void ClearLoginInfo(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(UserEmailKey);
        httpContext.Response.Cookies.Delete(UserDisplayNameKey);
        httpContext.Response.Cookies.Delete(AccessTokenKey);
        httpContext.Response.Cookies.Delete(RefreshTokenKey);
    }

    public static string? GetDefaultDomainFromCookie(HttpContext httpContext)
    {
        var domain = httpContext.Request.Cookies[DefaultDomainKey];
        return domain;
    }

    public static void WriteDefaultDomainToCookie(HttpContext httpContext, string domainId)
    {
        httpContext.Response.Cookies.Append(DefaultDomainKey, domainId);
    }

    public static UserInfoDto GetUserInfoFromCookie(HttpContext httpContext)
    {
        var email = httpContext.Request.Cookies[UserEmailKey];
        var displayName = httpContext.Request.Cookies[UserDisplayNameKey];

        return new UserInfoDto(email ?? "", displayName ?? "");
    }
}