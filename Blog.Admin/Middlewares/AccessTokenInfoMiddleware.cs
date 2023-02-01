using Blog.Admin.Helper;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;

namespace Blog.Admin.Middlewares;

public class AccessTokenInfoMiddleware : IMiddleware
{
    private readonly BlogService _blogService;

    public static readonly string AccessTokenInfoHttpContextItemsMiddlewareKey =
        "com.evrane.blog.admin.middleware.AccessTokenInfoMiddleware";

    public AccessTokenInfoMiddleware(BlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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

        await next(context);
    }
}

public static partial class Extensions
{
    public static IApplicationBuilder UseAccessTokenInfoMiddleware(this
        IApplicationBuilder app)
    {
        return app.UseMiddleware<AccessTokenInfoMiddleware>();
    }

    public static AccessTokenDto GetAccessTokenInfoFromHttpContextItems(this HttpContext context)
    {
        var accessToken = context.Items[AccessTokenInfoMiddleware.AccessTokenInfoHttpContextItemsMiddlewareKey];
        if (accessToken == null) return new AccessTokenDto(null, null);
        return (AccessTokenDto)accessToken;
    }
}