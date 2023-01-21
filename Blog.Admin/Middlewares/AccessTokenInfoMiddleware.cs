using Blog.Admin.Helper;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;

namespace Blog.Admin.Middlewares;

public class AccessTokenInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BlogService _blogService;
    public static readonly object AccessTokenInfoHttpContextItemsMiddlewareKey = new();

    public AccessTokenInfoMiddleware(RequestDelegate next, BlogService blogService)
    {
        _next = next;
        _blogService = blogService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var accessToken = CookieHelper.GetAccessTokenFromCookie(context);
        if (!string.IsNullOrEmpty(accessToken.AccessToken))
        {
            try
            {
                (var tokenUpdated, accessToken) =
                    await _blogService.EnsureAccessToken(accessToken);
                if (tokenUpdated)
                {
                    CookieHelper.WriteAccessTokenToCookie(context, accessToken);
                }
            }
            catch (HttpRequestException exception)
            {
                CookieHelper.ClearLoginInfo(context);
            }
        }

        context.Items[AccessTokenInfoHttpContextItemsMiddlewareKey] = accessToken;

        await _next(context);
    }
}

public static partial class HttpContextExtensions
{
    public static AccessTokenDto GetAccessTokenInfoFromHttpContextItems(this HttpContext context)
    {
        var accessToken = context.Items[AccessTokenInfoMiddleware.AccessTokenInfoHttpContextItemsMiddlewareKey];
        if (accessToken == null) return new AccessTokenDto(null, null);
        return (AccessTokenDto)accessToken;
    }
}