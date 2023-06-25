using System.Globalization;
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
}