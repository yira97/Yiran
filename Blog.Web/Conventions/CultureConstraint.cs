using System.Globalization;
using Microsoft.Extensions.Options;

namespace Blog.Web.Conventions;

public class CultureConstraint : IRouteConstraint
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public CultureConstraint(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        var cultures = _localizationOptions.Value.SupportedUICultures ?? new List<CultureInfo>();
        return cultures.Select(c => c.Name).Contains(values[routeKey]?.ToString());
    }
}