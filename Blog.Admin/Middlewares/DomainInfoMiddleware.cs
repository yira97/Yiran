using Blog.Admin.Helper;

namespace Blog.Admin.Middlewares;

public class DomainInfoMiddleware : IMiddleware
{
    public static readonly string DomainIdHttpContextItemsMiddlewareKey =
        "com.evrane.blog.admin.middleware.DomainInfoMiddleware";

    public DomainInfoMiddleware()
    {
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var domainIdQuery = context.Request.Query["domainId"].ToString();
        var domainIdCookie = CookieHelper.GetDefaultDomainFromCookie(context);

        // 如果　Query　中存在 - 使用 Query 中的存入 Items
        if (!string.IsNullOrEmpty(domainIdQuery))
        {
            context.Items[DomainIdHttpContextItemsMiddlewareKey] = domainIdQuery;
        }
        // 如果 Query 中不存在，使用 Cookie 中的存入 Items
        else if (!string.IsNullOrEmpty(domainIdCookie))
        {
            context.Items[DomainIdHttpContextItemsMiddlewareKey] = domainIdCookie;
        }

        await next(context);
    }
}

public static partial class Extensions
{
    public static IApplicationBuilder UseDomainInfoMiddleware(this
        IApplicationBuilder app)
    {
        return app.UseMiddleware<DomainInfoMiddleware>();
    }

    public static string? GetDomainIdFromHttpContextItems(this HttpContext context)
    {
        var domainId = context.Items[DomainInfoMiddleware.DomainIdHttpContextItemsMiddlewareKey];
        if (domainId == null) return null;
        return (string)domainId;
    }
}