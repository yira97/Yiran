using System.Globalization;
using Blog.Admin.Helper;
using Microsoft.AspNetCore.Localization;

namespace Blog.Admin.Middlewares;

public class CultureInfoMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (context.Request.Cookies.ContainsKey(CookieRequestCultureProvider.DefaultCookieName) == false)
        {
            CookieHelper.WriteCultureToCookie(context, CultureInfo.CurrentCulture);
        }
    }
}