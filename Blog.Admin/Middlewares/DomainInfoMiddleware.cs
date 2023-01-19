using Blog.Admin.Helper;
using Blog.Domain.Services.Client;

namespace Blog.Admin.Middlewares;

public class DomainInfoMiddleware
{
    private readonly RequestDelegate _next;
    public static readonly object DomainIdHttpContextItemsMiddlewareKey = new();

    public DomainInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 如果　Query　中存在 - 使用 Query 中的存入 Items
        var domainIdQuery = context.Request.Query["domainId"].ToString();
        var domainIdCookie = CookieHelper.GetDefaultDomainFromCookie(context);

        if (!string.IsNullOrEmpty(domainIdQuery))
        {
            context.Items[DomainIdHttpContextItemsMiddlewareKey] = domainIdQuery;
        }
        else if (!string.IsNullOrEmpty(domainIdCookie))
        {
            context.Items[DomainIdHttpContextItemsMiddlewareKey] = domainIdCookie;
        }

        await _next(context);
    }
}

public static class HttpContextExtensions
{
    public static string? GetDomainIdFromHttpContextItems(this HttpContext context)
    {
        var domainId = context.Items[DomainInfoMiddleware.DomainIdHttpContextItemsMiddlewareKey];
        if (domainId == null) return null;
        return (string)domainId;
    }
}