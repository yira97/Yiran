using Blog.Domain.Models;

namespace Blog.Admin.Helper;

public static class CookieHelper
{
    private const string AccessTokenKey = "X-Access-Token";
    private const string RefreshTokenKey = "X-Refresh-Token";

    public static AccessTokenDto GetAccessTokenFromCookie(HttpContext httpContrext)
    {
        var accessToken = httpContrext.Request.Cookies[AccessTokenKey];
        var refreshToken = httpContrext.Request.Cookies[RefreshTokenKey];

        return new AccessTokenDto(accessToken!, refreshToken!);
    }

    public static void WriteAccessTokenToCookie(HttpContext httpContext, AccessTokenDto accessToken)
    {
        httpContext.Response.Cookies.Append(AccessTokenKey, accessToken.AccessToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
        httpContext.Response.Cookies.Append(RefreshTokenKey, accessToken.RefreshToken,
            new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict });
    }
}