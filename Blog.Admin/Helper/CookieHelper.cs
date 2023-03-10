using System.Globalization;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Microsoft.AspNetCore.Localization;

namespace Blog.Admin.Helper;

public static class CookieHelper
{
    private const string AccessTokenKey = "X-Access-Token";
    private const string RefreshTokenKey = "X-Refresh-Token";
    private const string UserEmailKey = "User-Email";
    private const string UserNickNameKey = "User-NickName";
    private const string DefaultDomainKey = "Default-Domain";

    public static AccessTokenDto GetAccessTokenFromCookie(HttpContext httpContext)
    {
        var accessToken = httpContext.Request.Cookies[AccessTokenKey];
        var refreshToken = httpContext.Request.Cookies[RefreshTokenKey];

        return new AccessTokenDto(accessToken, refreshToken);
    }

    public static void WriteAccessTokenToCookie(HttpContext httpContext, AccessTokenDto accessToken)
    {
        if (accessToken.AccessToken != null)
        {
            httpContext.Response.Cookies.Append(AccessTokenKey, accessToken.AccessToken,
                new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        }

        if (accessToken.RefreshToken != null)
        {
            httpContext.Response.Cookies.Append(RefreshTokenKey, accessToken.RefreshToken,
                new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddYears(1) });
        }
    }

    public static void ClearLoginInfo(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(UserEmailKey);
        httpContext.Response.Cookies.Delete(UserNickNameKey);
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

    public static void WriteCultureToCookie(HttpContext httpContext, CultureInfo culture)
    {
        httpContext.Response.Cookies.Append
        (
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture))
        );
    }

    public static UserInfoDto GetUserInfoFromCookie(HttpContext httpContext)
    {
        var email = httpContext.Request.Cookies[UserEmailKey];
        var displayName = httpContext.Request.Cookies[UserNickNameKey];

        return new UserInfoDto
        {
            Email = email ?? string.Empty,
            NickName = displayName ?? string.Empty,
        };
    }

    public static void WriteUserInfoToCookie(HttpContext httpContext, UserInfoDto userInfoDto)
    {
        httpContext.Response.Cookies.Append(UserEmailKey, userInfoDto.Email,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });

        httpContext.Response.Cookies.Append(UserNickNameKey, userInfoDto.NickName,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
    }
}